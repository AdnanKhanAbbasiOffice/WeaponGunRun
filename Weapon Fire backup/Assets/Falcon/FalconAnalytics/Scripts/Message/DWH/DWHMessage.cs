using System;
using Falcon.FalconAnalytics.Scripts.Counters;
using Falcon.FalconAnalytics.Scripts.Message.Wrapper;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Data;
using Newtonsoft.Json;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public abstract class DwhMessage
    {
        public string abTestingValue;
        public string abTestingVariable;
        public string accountId;
        public string appVersion = FDeviceInfo.AppVersion;
        public string deviceId = FDeviceInfo.DeviceId;
        public string gameId = FDeviceInfo.GameId;
        public string gameName = FDeviceInfo.GameName;
        public string installDay = FTime.DateToString(RetentionCounter.FirstLoginDate);
        public int level = PlayerParams.MaxPassedLevel;
        public string platform = FDeviceInfo.Platform;
        public int retentionDay = RetentionCounter.Retention;
        public string sdkVersion = "2.2.4";
        public int apiId;
        public long clientCreateDate;

        protected DwhMessage(long clientCreateDate)
        {
            abTestingValue = TrimIfTooLong(PlayerParams.AbTestingValue, 50);
            abTestingVariable = TrimIfTooLong(PlayerParams.AbTestingVariable, 100);
            accountId = CheckStringLength(PlayerParams.AccountID, nameof(accountId),50);
            apiId = GetApiId();
            this.clientCreateDate = clientCreateDate;
        }

        private int GetApiId()
        {
            var appIdKey = GetType().Name + "app_id";
            int result = FData.Instance.Compute<int>(appIdKey, (hasKey, intVal) =>
            {
                if (!hasKey) return 0;
                return intVal + 1;
            });
            return result;
        }
        
        protected abstract string GetAPI();

        public virtual DataWrapper Wrap()
        {
            AnalyticLogger.Instance.Info(GetType().Name + " message created : " + JsonConvert.SerializeObject(this));
            return new DataWrapper(this, GetAPI());
        }

        #region Check Params

        protected string TrimIfTooLong(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            if (str.Length > maxLength)
            {
                return str.Substring(0, maxLength);
            }
            return str;
        }
        
        protected string CheckStringLength(string str, string fieldName, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
            {
                AnalyticLogger.Instance.Error(
                    $"Dwh Log invalid field: the string length of {fieldName} of {GetType().Name.Substring(3)} has empty input");
                return "";
            }

            if (str.Length > maxLength)
            {
                AnalyticLogger.Instance.Error(
                    $"Dwh Log invalid field: the string length of {fieldName} of {GetType().Name.Substring(3)} should only be at max at {maxLength}, the input {str} has the length of {str.Length}, substring-ing it down");
                return str.Substring(0, maxLength);
            }
            return str;
        }

        protected int CheckNumberNonNegative(int i, string fieldName)
        {
            if (i < 0)
                AnalyticLogger.Instance.Error(
                    $"Dwh Log invalid field: the value of field {fieldName} of {GetType().Name.Substring(3)} must be non-negative, input value '{i}'");
            return i;
        }
        protected long CheckNumberNonNegative(long i, string fieldName)
        {
            if (i < 0)
                AnalyticLogger.Instance.Error(
                    $"Dwh Log invalid field: the value of field {fieldName} of {GetType().Name.Substring(3)} must be non-negative, input value '{i}'");
            return i;
        }
        protected float CheckNumberNonNegative(float i, string fieldName)
        {
            if (i < 0)
                AnalyticLogger.Instance.Error(
                    $"Dwh Log invalid field: the value of field {fieldName} of {GetType().Name.Substring(3)} must be non-negative, input value '{i}'");
            return i;
        }
        
        protected double CheckNumberNonNegative(double i, string fieldName)
        {
            if (i < 0)
                AnalyticLogger.Instance.Error(
                    $"Dwh Log invalid field: the value of field {fieldName} of {GetType().Name.Substring(3)} must be non-negative, input value '{i}'");
            return i;
        }
        
        protected decimal CheckNumberNonNegative(decimal i, string fieldName)
        {
            if (i < 0)
                AnalyticLogger.Instance.Error(
                    $"Dwh Log invalid field: the value of field {fieldName} of {GetType().Name.Substring(3)} must be non-negative, input value '{i}'");
            return i;
        }
        #endregion
    }
}
