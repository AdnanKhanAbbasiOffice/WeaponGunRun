using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Falcon.FalconCore.Scripts.Exceptions;
using Falcon.FalconCore.Scripts.Interfaces;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Logs;
using Falcon.FalconCore.Scripts.Utils.Sequences.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif

namespace Falcon.FalconCore.Scripts
{
    public class FalconMain : MonoBehaviour
    {
        #region Singleton

        private static FalconMain _instance;

        public static FalconMain Instance
        {
            get
            {
                if (_instance == null)
                {
                    var gObject = GameObject.Find("Falcon");
                    if (gObject == null) gObject = new GameObject("Falcon");

                    _instance = gObject.GetComponent<FalconMain>() == null
                        ? gObject.AddComponent<FalconMain>()
                        : gObject.GetComponent<FalconMain>();

                    _instance.enabled = true;
                    if (Application.isPlaying) DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        #endregion

        #region Init

        private static readonly List<IFEssential> Essentials = new List<IFEssential>();

        public static ExecState InitState { get; private set; } = ExecState.NotStarted;

        public static bool InitComplete => InitState == ExecState.Succeed;

        public static int? MainThreadId { get; private set; }

        public static event EventHandler OnInitComplete;

        /// <summary>
        /// Initialize the SDk
        /// </summary>
        /// <remarks>
        /// From SDK version 2.2.0, this function is no longer needed to be called manually by the user
        /// </remarks>
        /// <exception cref="FSdkException">If function not called from the main thread, only be thrown if running in editor</exception>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            if (ExecStates.CanStart(InitState))
            {
#if UNITY_EDITOR
                if (!InternalEditorUtility.CurrentThreadIsMainThread())
                    throw new FSdkException("FalconMain.Init() can only be called from the main thread");
#endif
                Instance.StartCoroutine(new SequenceWrap(InitIEnumerator(), e =>
                {
                    CoreLogger.Instance.Error(e);
                    InitState = ExecState.Failed;
                }));
            }
        }

        private static IEnumerator InitIEnumerator()
        {
            MainThreadId = Thread.CurrentThread.ManagedThreadId;
            InitState = ExecState.Processing;

            CoreLogger.Instance.Info("Sdk Initialize Started");

            yield return CallEssentials();

            yield return CallInit();

            CoreLogger.Instance.Info("Initialize complete");

            InitState = ExecState.Succeed;
            OnInitComplete?.Invoke(null, EventArgs.Empty);
        }

        private static IEnumerator CallEssentials()
        {
            Essentials.Clear();
            var types = from t in AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                        where t.GetInterfaces().Contains(typeof(IFEssential))
                              && t.GetConstructor(Type.EmptyTypes) != null
                        select t;

            foreach (var type in types)
            {
                CoreLogger.Instance.Info(type + " initializing");
                Essentials.Add(GetInstance<IFEssential>(type));
                CoreLogger.Instance.Info(type + " init complete");
                yield return null;
            }
        }

        private static IEnumerator CallInit()
        {
            var types = from t in AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                        where t.GetInterfaces().Contains(typeof(IFInit))
                              && t.GetConstructor(Type.EmptyTypes) != null
                        select t;

            foreach (var type in types)
            {
                var instance = GetInstance<IFInit>(type);

                CoreLogger.Instance.Info(instance.GetType() + " initializing");
                yield return instance.Init();
                CoreLogger.Instance.Info(instance.GetType() + " init complete");
            }
        }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        private static T GetInstance<T>(Type type) where T : class, IFMainInit
        {
            T instance;
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                instance = Instance.AddIfNotExist(type) as T;
            }

            else
            {
                instance = Activator.CreateInstance(type) as T;
            }

            if (instance != null) return instance;
            throw new FSdkException("Failed to created instance of type: " + type);
        }

        #endregion

        #region Expand Methods

        public static bool ApplicationRunning { get; private set; }

        public T AddIfNotExist<T>() where T : MonoBehaviour
        {
            var val = gameObject.GetComponent<T>();
            if (val == null) val = gameObject.AddComponent<T>();
            return val;
        }

        public Component AddIfNotExist(Type type)
        {
            var val = gameObject.GetComponent(type);
            if (val == null) val = gameObject.AddComponent(type);
            return val;
        }

        #endregion

        #region Events

        public event EventHandler OnGameStop;
        public event EventHandler OnGameContinue;
        public event EventHandler OnUpdate;

        private bool gameStop;

        private void Awake()
        {
            ApplicationRunning = true;
        }

        public void Update()
        {
            try
            {
                OnUpdate?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                CoreLogger.Instance.Error(e);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            MainThreadId = Thread.CurrentThread.ManagedThreadId;

            if (Application.isPlaying && !hasFocus)
                CheckGameStop();
            else if (Application.isPlaying) CheckGameContinue();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            MainThreadId = Thread.CurrentThread.ManagedThreadId;

            if (Application.isPlaying && pauseStatus)
                CheckGameStop();
            else if (Application.isPlaying) CheckGameContinue();
        }

        private void OnApplicationQuit()
        {
            MainThreadId = Thread.CurrentThread.ManagedThreadId;
            ApplicationRunning = false;

            CheckGameStop();
        }

        private void CheckGameStop()
        {
            if (!gameStop)
                gameStop = true;
            else
                return;

            CoreLogger.Instance.Info("On Game Stop");

            try
            {
                OnGameStop?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                CoreLogger.Instance.Error(e);
            }

            foreach (var essential in Essentials)
                try
                {
                    essential.OnPostStop();
                }
                catch (Exception e)
                {
                    CoreLogger.Instance.Error(e);
                }
        }

        private void CheckGameContinue()
        {
            if (gameStop)
                gameStop = false;
            else
                return;
            CoreLogger.Instance.Info("On Game Continue");

            foreach (var essential in Essentials)
                try
                {
                    essential.OnPreContinue();
                }
                catch (Exception e)
                {
                    CoreLogger.Instance.Error(e);
                }

            try
            {
                OnGameContinue?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                CoreLogger.Instance.Error(e);
            }
        }

        #endregion
    }
}