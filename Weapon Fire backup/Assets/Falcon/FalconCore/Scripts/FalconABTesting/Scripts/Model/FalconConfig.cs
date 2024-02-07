using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Payload;
using Falcon.FalconCore.Scripts.Interfaces;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Data;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts;
using Falcon.FalconCore.Scripts.Utils.Logs;
using UnityEngine.Scripting;

namespace Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model
{
    public abstract class FalconConfig
    {
        private const string Config = "FALCON_CONFIG";
        private static readonly ConcurrentDictionary<Type, FalconConfig> Configs =
            new ConcurrentDictionary<Type, FalconConfig>();

        public static ExecState UpdateFromNet { get; private set; } = ExecState.NotStarted;
        
        public static event EventHandler OnUpdateFromNet;

        public static List<ConfigObject> TestingConfigs { get; private set; }

        public static string RunningAbTesting { get; private set; }

        public static T Instance<T>() where T : FalconConfig, new()
        {
            InitConfig();
            FalconConfig instance;

            if (Configs.TryGetValue(typeof(T), out instance) && instance != null) 
                return (T)instance;
            return new T();
        }

        private static bool _inited;

        private static void InitConfig()
        {
            if(_inited) return;
            
            UpdateFromNet = ExecState.Processing;

            var configFileTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(FalconConfig)));


            foreach (var configFileType in configFileTypes)
                Configs[configFileType] = (FalconConfig)Activator.CreateInstance(configFileType);

            FData.Instance.Compute<ReceiveConfig>(Config, (hasKey,config) =>
            {
                if (hasKey && config != null)
                {
                    UpdateFromReceivedConfigObjList(config.Configs);
                    RunningAbTesting = config.RunningAbTesting;
                }
                return config;
            });
            
            _inited = true;
        }
        
        private static void UpdateFromReceivedConfigObjList(ICollection<ReceiveConfigObject> configObjects)
        {
            foreach (var falconConfig in Configs.Select(config => config.Value))
            {
                foreach (ReceiveConfigObject configObject in configObjects)
                    try
                    {
                        var info = falconConfig.GetType().GetField(configObject.Name,
                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
                        if (info != null) info.SetValue(falconConfig, Convert.ChangeType(configObject.Value, info.FieldType));
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
            }

            List<ConfigObject> list = new List<ConfigObject>();
            foreach (var configObject in configObjects)
                if (configObject.AbTesting) list.Add(new ConfigObject(configObject));
            TestingConfigs = list;
        }

        [Preserve]
        private sealed class FConfigInit : IFInit
        {
            [Preserve]
            public FConfigInit()
            {
            }

            public IEnumerator Init()
            {
                InitConfig();
                UnitAction webPull = new UnitAction(UpdateFromWeb);
                webPull.Schedule();
                while (!webPull.Done)
                {
                    yield return null;
                }
            }

            private void UpdateFromWeb()
            {
                var sendConfig = new SendConfig();

                try
                {
                    ReceiveConfig receiveConfig = sendConfig.Connect(); 
                    UpdateFromReceivedConfigObjList(receiveConfig.Configs);
                    RunningAbTesting = receiveConfig.RunningAbTesting;
                    UpdateFromNet = ExecState.Succeed;
                    FData.Instance.Save(Config, receiveConfig);
                    
                    new MainThreadAction(() =>
                    {
                        OnUpdateFromNet?.Invoke(null, EventArgs.Empty);
                    }).Schedule();
                }
                catch (Exception e)
                {
                    CoreLogger.Instance.Error(e);
                    UpdateFromNet = ExecState.Failed;
                }
            }
        }
    }
}