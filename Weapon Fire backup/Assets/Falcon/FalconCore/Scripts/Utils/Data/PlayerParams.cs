using System;
using System.Globalization;
using System.Text;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;

namespace Falcon.FalconCore.Scripts.Utils.Data
{
    public static class PlayerParams
    {
        private const string AnalyticDataPrefix = "Analytic_SDK_Data_";
        private const string AccountIDKey = AnalyticDataPrefix + "Account_ID_Key";

        private const string FirstLoginDateKey = AnalyticDataPrefix + "First_Login_Date";

        private const string MaxLevelKey = AnalyticDataPrefix + "Max_Level";
        private const string InterstitialCountKey = AnalyticDataPrefix + "Interstitial_Ad_Count";
        private const string RewardCountKey = AnalyticDataPrefix + "Reward_Ad_Count";
        private const string SessionIDKey = "Session_Count";

        private static string _abTestVariable;
        private static string _abTestValue;
        
        public static string AccountID
        {
            get { return FData.Instance.GetOrDefault(AccountIDKey, FDeviceInfo.DeviceId); }
            set { FData.Instance.Save(AccountIDKey, value); }
        }

        public static string AbTestingVariable
        {
            get
            {
                if (_abTestVariable != null) return _abTestVariable;

                if (FalconConfig.RunningAbTesting == null) return "";
                _abTestVariable = FalconConfig.RunningAbTesting;
                return _abTestVariable;
            }
        }
        
        public static string AbTestingValue
        {
            get
            {
                if (_abTestValue == null)
                {
                        if (FalconConfig.TestingConfigs == null || FalconConfig.TestingConfigs.Count <= 0) return "";
                        StringBuilder builder = new StringBuilder();
                        foreach (var configObj in FalconConfig.TestingConfigs)
                            builder.Append(Convert.ToString(configObj.Name, CultureInfo.InvariantCulture))
                                .Append(":")
                                .Append(Convert.ToString(configObj.Value, CultureInfo.InvariantCulture))
                                .Append("_");
                        if (builder.Length > 0) builder.Length--;

                        _abTestValue = builder.ToString();

                }

                return _abTestValue;
            }
        }

        public static DateTime FirstLoginDate
        {
            get { return FData.Instance.GetOrDefault(FirstLoginDateKey, FDeviceInfo.FirstLogInDateTime); }
            set
            {
                FData.Instance.Save(FirstLoginDateKey, value);
            }
        }
        
        public static int MaxPassedLevel
        {
            get { return FData.Instance.GetOrSet(MaxLevelKey, 0); }
            set
            {
                if(value <= MaxPassedLevel) return;
                FData.Instance.Save(MaxLevelKey, value);
            }
        }

        public static int InterstitialAdCount
        {
            get { return FData.Instance.GetOrSet(InterstitialCountKey, 0); }
            set { FData.Instance.Save(InterstitialCountKey, value); }
        }

        public static int RewardAdCount
        {
            get { return FData.Instance.GetOrSet(RewardCountKey, 0); }
            set { FData.Instance.Save(RewardCountKey, value); }
        }

        private static int _sessionId = -1;

        public static int SessionId
        {
            get
            {
                if (_sessionId == -1)
                {
                    _sessionId = FData.Instance.GetOrSet(SessionIDKey, 0) + 1;
                    FData.Instance.Save(SessionIDKey, _sessionId);
                }

                return _sessionId;
            }
        }
    }
}