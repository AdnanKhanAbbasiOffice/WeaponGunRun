using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Falcon.FalconCore.Editor.FPlugins.BitBucketObjs;
using Falcon.FalconCore.Editor.Utils;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts;
using Falcon.FalconCore.Scripts.Utils.Logs;
using Falcon.FalconCore.Scripts.Utils.Sequences.Core;
using Falcon.FalconCore.Scripts.Utils.Sequences.Editor;
using Falcon.FalconCore.Scripts.Utils.Sequences.Entity;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Falcon.FalconCore.Editor.FPlugins
{
    public class FPlugin
    {
        private const string UnityPackageExtension = ".unitypackage";

        #region Static

        public static ExecState PullState { get; private set; } = ExecState.NotStarted;

        public static int RemotePluginCount { get; private set; }

        private static readonly Dictionary<string, FPlugin> NameToPlugins = new Dictionary<string, FPlugin>();

        public static IEnumerator<FPlugin> GetAsync(String pluginName)
        {

            if (ExecStates.CanStart(PullState))
            {
                SequenceWrap init = new SequenceWrap(Init());
                while (init.MoveNext()) yield return null;
            }

            while (PullState != ExecState.Succeed)
            {
                yield return null;
            }

            yield return NameToPlugins[pluginName];
        }

        public static IEnumerator<ICollection<FPlugin>> GetAllAsync()
        {

            if (ExecStates.CanStart(PullState))
            {
                SequenceWrap wrap = new SequenceWrap(Init());
                while (wrap.MoveNext()) yield return null;
            }

            while (PullState != ExecState.Succeed)
            {
                yield return null;
            }

            yield return NameToPlugins.Values;
        }

        public static ICollection<FPlugin> GetAllSync()
        {
            if (ExecStates.CanStart(PullState))
            {
                new EditorSequence(Init()).Start();
            }

            return NameToPlugins.Values;
        }

        #endregion

        #region NetPull

        private const string SdkUrl =
            "https://api.bitbucket.org/2.0/repositories/falcongame/falcon-unity-sdk/src/master/Assets/Falcon/Release/";

        public static IEnumerator Init()
        {
            if (!ExecStates.CanStart(PullState)) yield break;
            PullState = ExecState.Processing;
            var getAllPlugin = new BitBucCall(SdkUrl);

            yield return getAllPlugin;

            var pluginLinks = getAllPlugin.Current;

            RemotePluginCount = 0;

            foreach (var value in pluginLinks)
            {
                if (value.Path != null && !value.Path.EndsWith(".meta"))
                {
                    RemotePluginCount++;
                    var pluginBuild = new FPluginBuilder(value);
                    yield return pluginBuild;
                    NameToPlugins[pluginBuild.Current.PluginName] = pluginBuild.Current;
                }
            }
            PullState = ExecState.Succeed;
        }

        public static void Clear()
        {
            PullState = ExecState.NotStarted;

            RemotePluginCount = 0;

            NameToPlugins.Clear();
        }

        #endregion


        private FPlugin()
        {
        }

        public FPluginMeta InstalledConfig { get; private set; }

        public string PluginName { get; private set; }

        public FPluginMeta RemoteConfig { get; private set; }

        public string PluginUrl { get; private set; }
        public string PluginIronSource { get; private set; }
        public string PluginMax { get; private set; }
        public string PluginGG { get; private set; }

        public bool Installed { get; private set; }

        public string InstalledDirectory { get; private set; }
        public string InstalledIrsDirectory { get; private set; }
        public string InstalledMaxDirectory { get; private set; }
        public string InstalledGGDirectory { get; private set; }

        public IEnumerator Install()
        {
            var requireJson = RemoteConfig.Require;
            var requirePlugins = new List<FPlugin>();
            if (requireJson != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in requireJson)
                {
                    SeqWrap<FPlugin> plugin = new SeqWrap<FPlugin>(GetAsync(keyValuePair.Key));

                    yield return plugin;

                    if (string.CompareOrdinal(plugin.Current.InstalledConfig.Version, keyValuePair.Value) < 0)
                        requirePlugins.Add(plugin.Current);
                }
            }
            if (requirePlugins.Count == 0)
            {
                yield return UnsafeInstall();
                yield break;
            }

            var requirePluginString = new StringBuilder();
            requirePluginString
                .Append("The following plugins are required for" + PluginName + ":").AppendLine();
            foreach (var plugin in requirePlugins)
                requirePluginString.Append("  - ").Append(plugin.PluginName).AppendLine();

            requirePluginString.Append("Please install/update them first!");

            EditorUtility.DisplayDialog("Additional plugin require!!!",
                requirePluginString.ToString(), "Ok");
        }

        void RemoveMaxSDK()
        {
            //remove thirparty/FalconMax
            if (InstalledMaxDirectory != null)
            {
                FileUtil.DeleteFileOrDirectory(InstalledMaxDirectory + ".meta");
                FileUtil.DeleteFileOrDirectory(InstalledMaxDirectory);
                InstalledMaxDirectory = null;
            }
        }

        void RemoveIronSourceSDK()
        {
            //remove thirdparty/FalconIronSource
            if (InstalledIrsDirectory != null)
            {
                FileUtil.DeleteFileOrDirectory(InstalledIrsDirectory + ".meta");
                FileUtil.DeleteFileOrDirectory(InstalledIrsDirectory);
                InstalledIrsDirectory = null;
            }
            //remove plugins/IronSource.androidlib
            FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Plugins", "Android", "IronSource.androidlib.meta"));
            FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Plugins", "Android", "IronSource.androidlib"));
        }

        void RemoveGoogleMobileAdsSDK()
        {
            if (!IsUseUMP())
            {
                //remove thirparty/FalconGoogleMobileAds
                if (InstalledGGDirectory != null)
                {
                    FileUtil.DeleteFileOrDirectory(InstalledGGDirectory + ".meta");
                    FileUtil.DeleteFileOrDirectory(InstalledGGDirectory);
                    InstalledGGDirectory = null;
                }
            }
            else
            {
                FileUtil.DeleteFileOrDirectory(Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName + "/ThirdParty/FalconGoogleMobileAds", "Editor.meta"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName + "/ThirdParty/FalconGoogleMobileAds", "Editor"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName + "/ThirdParty/FalconGoogleMobileAds", "FalconGoogleMobileAdsConstant.cs.meta"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName + "/ThirdParty/FalconGoogleMobileAds", "FalconGoogleMobileAdsConstant.cs"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName + "/ThirdParty/FalconGoogleMobileAds", "FalconGoogleMobileAdsMediation.cs.meta"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName + "/ThirdParty/FalconGoogleMobileAds", "FalconGoogleMobileAdsMediation.cs"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName + "/ThirdParty/FalconGoogleMobileAds", "FalconGoogleMobileAdsSettings.cs.meta"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName + "/ThirdParty/FalconGoogleMobileAds", "FalconGoogleMobileAdsSettings.cs"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName + "/ThirdParty/FalconGoogleMobileAds", "GoogleMobileAdsService.cs.meta"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName + "/ThirdParty/FalconGoogleMobileAds", "GoogleMobileAdsService.cs"));
            }
            //remove plugins/GoogleMobileAdsPlugin.androidlib + googlemobileads-unity.aar
            FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Plugins", "Android", "GoogleMobileAdsPlugin.androidlib.meta"));
            FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Plugins", "Android", "GoogleMobileAdsPlugin.androidlib"));
            FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Plugins", "Android", "googlemobileads-unity.aar.meta"));
            FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Plugins", "Android", "googlemobileads-unity.aar"));
        }

        public IEnumerator InstallIronsource()
        {
            RemoveMaxSDK();
            RemoveGoogleMobileAdsSDK();
            var tempFolder = Application.dataPath + "/../Temp/Ironsource" + UnityPackageExtension;

            InstalledConfig.VersionIrs = RemoteConfig.VersionIrs;
            InstalledIrsDirectory = Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName, "ThirdParty", "FalconIronSource");

            var fileGetRequest = new FileGetRequest(PluginIronSource, tempFolder);
            new Thread(fileGetRequest.Invoke).Start();
            while (!fileGetRequest.Done) yield return null;

            CoreLogger.Instance.Info("Downloading Ironsource");
            CoreLogger.Instance.Info("Downloading complete, preparing to import");
            AssetDatabase.ImportPackage(tempFolder, true);
        }

        public IEnumerator InstallMax()
        {
            RemoveGoogleMobileAdsSDK();
            RemoveIronSourceSDK();
            var tempFolder = Application.dataPath + "/../Temp/Max" + UnityPackageExtension;

            InstalledConfig.VersionMax = RemoteConfig.VersionMax;
            InstalledMaxDirectory = Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName, "ThirdParty", "FalconMax");

            var fileGetRequest = new FileGetRequest(PluginMax, tempFolder);
            new Thread(fileGetRequest.Invoke).Start();
            while (!fileGetRequest.Done) yield return null;

            CoreLogger.Instance.Info("Downloading MaxSDK");
            CoreLogger.Instance.Info("Downloading complete, preparing to import");
            AssetDatabase.ImportPackage(tempFolder, true);
        }

        public IEnumerator InstallGG()
        {
            RemoveIronSourceSDK();
            RemoveMaxSDK();
            var tempFolder = Application.dataPath + "/../Temp/GoogleMobileAds" + UnityPackageExtension;

            InstalledConfig.VersionGG = RemoteConfig.VersionGG;
            InstalledGGDirectory = Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName, "ThirdParty", "FalconGoogleMobileAds");

            var fileGetRequest = new FileGetRequest(PluginGG, tempFolder);
            new Thread(fileGetRequest.Invoke).Start();
            while (!fileGetRequest.Done) yield return null;

            CoreLogger.Instance.Info("Downloading GoogleMobileAds");
            CoreLogger.Instance.Info("Downloading complete, preparing to import");
            AssetDatabase.ImportPackage(tempFolder, true);
        }

        private IEnumerator UnsafeInstall()
        {
            var tempFolder = Application.dataPath + "/../Temp/" + PluginName + UnityPackageExtension;

            InstalledConfig = RemoteConfig;
            Installed = true;
            InstalledDirectory = Path.Combine(FalconCoreFileUtils.GetFalconPluginFolder(), PluginName);

            var fileGetRequest = new FileGetRequest(PluginUrl, tempFolder);
            new Thread(fileGetRequest.Invoke).Start();
            while (!fileGetRequest.Done) yield return null;

            CoreLogger.Instance.Info("Downloading " + PluginName);
            CoreLogger.Instance.Info("Downloading complete, preparing to import");
            AssetDatabase.ImportPackage(tempFolder, true);
        }

        public void UnInstall()
        {
            CoreLogger.Instance.Info("Start uninstallation");
            if (IsFalconUMP())
            {
                if (!IsUseGG())
                {
                    if (!IsUseIrs() && !IsUseMax())
                    {
                        FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Falcon/FalconMediation.meta"));
                        FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Falcon/FalconMediation"));
                    }
                    else
                    {
                        FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Falcon/FalconMediation/ThirdParty/FalconGoogleMobileAds.meta"));
                        FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Falcon/FalconMediation/ThirdParty/FalconGoogleMobileAds"));
                    }
                }
            }
            if (IsFalconMediation() && IsUseUMP())
            {
                FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Falcon/FalconMediation/Core.meta"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Falcon/FalconMediation/Core"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Falcon/FalconMediation/config.txt.meta"));
                FileUtil.DeleteFileOrDirectory(Path.Combine(Application.dataPath, "Falcon/FalconMediation/config.txt"));
                RemoveGoogleMobileAdsSDK();
                RemoveIronSourceSDK();
                RemoveMaxSDK();
            }
            else
            {
                FileUtil.DeleteFileOrDirectory(InstalledDirectory + ".meta");
                FileUtil.DeleteFileOrDirectory(InstalledDirectory);
            }
            Installed = false;
            InstalledConfig = null;
            InstalledDirectory = null;
            AssetDatabase.Refresh();
        }

        public bool IsFalconCore()
        {
            return string.CompareOrdinal(PluginName, "FalconCore") == 0;
        }

        public bool IsFalconMediation()
        {
            return string.CompareOrdinal(PluginName, "FalconMediation") == 0;
        }

        public bool IsFalconUMP()
        {
            return string.CompareOrdinal(PluginName, "FalconGoogleUMP") == 0;
        }

        public bool IsUseIrs()
        {
            if (!Directory.Exists(Application.dataPath + "/Falcon/FalconMediation/ThirdParty")) return false;
            string[] directory =
                    Directory.GetDirectories(Application.dataPath + "/Falcon/FalconMediation/ThirdParty", "FalconIronSource", SearchOption.AllDirectories);
            return directory.Length != 0;
        }

        public bool IsUseMax()
        {
            if (!Directory.Exists(Application.dataPath + "/Falcon/FalconMediation/ThirdParty")) return false;
            string[] directory =
                    Directory.GetDirectories(Application.dataPath + "/Falcon/FalconMediation/ThirdParty", "FalconMax", SearchOption.AllDirectories);
            return directory.Length != 0;
        }

        public bool IsUseGG()
        {
            if (Directory.Exists(Application.dataPath + "/Falcon/FalconMediation/ThirdParty/FalconGoogleMobileAds/Editor")) return true;
            return false;
        }

        public bool IsUseUMP()
        {
            if (Directory.Exists(Application.dataPath + "/Falcon/FalconGoogleUMP")) return true;
            return false;
        }

        public bool InstalledNewest()
        {
            if (string.CompareOrdinal(InstalledConfig.Version, RemoteConfig.Version) < 0) return false;
            if (!IsFalconMediation()) return true;
            if (IsUseIrs() && string.CompareOrdinal(InstalledConfig.VersionIrs, RemoteConfig.VersionIrs) < 0) return false;
            if (IsUseMax() && string.CompareOrdinal(InstalledConfig.VersionMax, RemoteConfig.VersionMax) < 0) return false;
            return true;
        }

        private class FPluginBuilder : Sequence<FPlugin>
        {
            private readonly BitBucObj obj;

            public FPluginBuilder(BitBucObj obj)
            {
                this.obj = obj;
            }

            protected override IEnumerator<FPlugin> EnumeratorT()
            {
                FPlugin plugin = new FPlugin();
                string[] tokens = obj.Path.Split('/');
                plugin.PluginName = tokens[tokens.Length - 1];

                BitBucObj remoteConfigLink = null;
                HashSet<BitBucObj> pluginVersions = new HashSet<BitBucObj>();
                BitBucCall getAllVersion = new BitBucCall(obj.Links.Self.HRef);

                while (getAllVersion.MoveNext()) yield return null;

                foreach (BitBucObj value in getAllVersion.Current)
                {
                    var href = value.Links.Self.HRef;
                    if (href != null && href.EndsWith("config.txt"))
                    {
                        remoteConfigLink = value;
                    }
                    else if (href != null && !href.EndsWith(".meta"))
                    {
                        pluginVersions.Add(value);
                    }
                }

                HttpSequence configRead = new HttpSequence(HttpMethod.Get, remoteConfigLink.Links.Self.HRef);
                while (configRead.MoveNext()) yield return null;

                plugin.RemoteConfig = JsonConvert.DeserializeObject<FPluginMeta>(configRead.Current);
                foreach (var url in pluginVersions)
                {
                    if (plugin.PluginName.Equals("FalconMediation"))
                    {
                        if (url.Links.Self.HRef.EndsWith("IronSource-" + plugin.RemoteConfig.VersionIrs + UnityPackageExtension))
                        {
                            plugin.PluginIronSource = url.Links.Self.HRef;
                        }
                        if (url.Links.Self.HRef.EndsWith("MaxSdk-" + plugin.RemoteConfig.VersionMax + UnityPackageExtension))
                        {
                            plugin.PluginMax = url.Links.Self.HRef;
                        }
                        if (url.Links.Self.HRef.EndsWith("GoogleMobileAds-" + plugin.RemoteConfig.VersionGG + UnityPackageExtension))
                        {
                            plugin.PluginGG = url.Links.Self.HRef;
                        }
                    }
                    if (url.Path.EndsWith(plugin.PluginName + "-" + plugin.RemoteConfig.Version + UnityPackageExtension))
                    {
                        plugin.PluginUrl = url.Links.Self.HRef;
                    }
                }
                if (plugin.PluginUrl == null) plugin.PluginUrl = pluginVersions.First().Links.Self.HRef;

                UpdateInstalledConfig(plugin);

                yield return plugin;
            }

            private void UpdateInstalledConfig(FPlugin plugin)
            {
                string[] directory =
                    Directory.GetDirectories(Application.dataPath, plugin.PluginName, SearchOption.AllDirectories);

                if (directory.Length == 0) plugin.Installed = false;
                else
                    try
                    {
                        plugin.Installed = true;
                        plugin.InstalledDirectory = directory[0].Contains("Release") ? directory[1] : directory[0];
                        if (plugin.IsFalconMediation())
                        {
                            plugin.InstalledIrsDirectory = plugin.InstalledDirectory + "/ThirdParty/FalconIronSource";
                            plugin.InstalledMaxDirectory = plugin.InstalledDirectory + "/ThirdParty/FalconMax";
                            plugin.InstalledGGDirectory = plugin.InstalledDirectory + "/ThirdParty/FalconGoogleMobileAds";
                        }
                        plugin.InstalledConfig = JsonConvert.DeserializeObject<FPluginMeta>(File.ReadAllText(
                            plugin.InstalledDirectory + Path.DirectorySeparatorChar + "config.txt"));
                    }
                    catch (Exception)
                    {
                        plugin.Installed = false;
                    }
            }
        }


    }

    public class FPluginMeta
    {
        [JsonProperty(PropertyName = "require")]
        public Dictionary<string, string> Require;

        [JsonProperty(PropertyName = "version")]
        public string Version;

        [JsonProperty(PropertyName = "versionMax")]
        public string VersionMax;

        [JsonProperty(PropertyName = "versionIrs")]
        public string VersionIrs;

        [JsonProperty(PropertyName = "versionGG")]
        public string VersionGG;
    }
}