using System;
using UnityEngine;
#if UNITY_IOS
using System.Globalization;
using Falcon;
using Newtonsoft.Json;
using UnityEngine.iOS;
#endif

namespace Falcon.FalconCore.Scripts.Utils.Data
{
    public static class FDeviceInfo
    {
        private const string FirstLoginDateKey = "FIRST_DATE";
        private const string FirstLoginMillisKey = "CREATE_DATE";
        public static string GameId{get;private set;}
        public static string GameName{get;private set;}
        public static string Platform{get;private set;}
        public static string AppVersion{get;private set;}
        public static string SdkCoreVersion{get;private set;}
        public static string DeviceName{get;private set;}
        public static string DeviceId{get;private set;}
        public static DateTime FirstLogInDateTime{get;private set;}

        public static long FirstLogInMillis { get; private set; }

        static FDeviceInfo()
        {
            OnStart();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnStart()
        {
            GameId = Application.identifier.ToLower();
            GameName = Application.productName.ToLower();
            #if UNITY_IOS 
            Platform = "ios";
#elif UNITY_ANDROID
            Platform = "android";
#else
            Platform = Application.platform.ToString().ToLower();
#endif
            AppVersion = Application.version.ToLower();
            SdkCoreVersion = "2.2.4";
            DeviceName = SystemInfo.deviceName.ToLower();
            DeviceId = GetDeviceId();
            FirstLogInDateTime = FData.Instance.GetOrSet(FirstLoginDateKey, DateTime.Now);
            FirstLogInMillis = FData.Instance.GetOrSet(FirstLoginMillisKey, FTime.CurrentTimeMillis());
        }

        private static string GetDeviceId()
        {
#if UNITY_IOS
        string key = "falcon_" + Application.identifier + "_uuid";
        string uuid = UUIDiOS.GetKeyChainValue(key);

        if (!string.IsNullOrEmpty(uuid))
        {
            return uuid;
        }
        else
        {
            uuid = Guid.NewGuid().ToString();
            UUIDiOS.SaveKeyChainValue(key, uuid);
            return uuid;
        } 
#else 
            return SystemInfo.deviceUniqueIdentifier;
#endif
        }
    }
}