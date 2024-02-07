using System;
using System.Linq;
using System.Net.Http;
using Falcon.FalconAnalytics.Scripts.Message.DWH;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.Wrapper
{
    public class DataWrapper
    {
        private static ActionLogConfig _config;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void AssignConfigAfterInit()
        {
            FalconConfig.OnUpdateFromNet += (sender, args) =>
            {
                _config = FalconConfig.Instance<ActionLogConfig>();
            };
        }
        
        public readonly String MessageType;
        public readonly string URL;
        [JsonProperty(PropertyName = "data")] public string Data;

        private int failCount;

        [Preserve]
        [JsonConstructor]
        public DataWrapper(String messageType, string url, string data, int failCount)
        {
            MessageType = messageType;
            URL = url;
            Data = data;
            this.failCount = failCount;
        }

        [Preserve]
        public DataWrapper(DwhMessage message, string url)
        {
            MessageType = message.GetType().Name;
            Data = JsonConvert.SerializeObject(message);
            URL = url;
            failCount = 0;
        }

        public void Send()
        {
            if (MessageType != nameof(DwhActionLog))
            {
                NormalSend();
                return;
            }
            if (_config != null && _config.fCoreAnalyticActionLogEnable)
            {
                NormalSend();
            }
            else
            {
                AnalyticLogger.Instance.Error("Action log not enabled, log not send to server");
            }
            
        }

        private void NormalSend()
        {
            HttpRequest request = new HttpRequest
            {
                RequestType = HttpMethod.Post,
                URL = URL,
                JsonBody = JsonConvert.SerializeObject(this),
                Timeout = TimeSpan.FromSeconds(60)
            };
            request.Invoke();
            if (string.IsNullOrEmpty(request.Result))
            {
                AnalyticLogger.Instance.Error(request.Exception);
            }
            else
            {
                var response = new string(request.Result.Where(c => !char.IsControl(c)).ToArray());
            
                if (string.Equals(response, "SS", StringComparison.Ordinal) ||
                    string.Equals(response, "SS\n", StringComparison.Ordinal))
                    AnalyticLogger.Instance.Info(MessageType.Substring(3) + " has been sent successfully");
                else
                    AnalyticLogger.Instance.Error(MessageType.Substring(3) +
                                                    " has been sent failed with the response of: " + response);
            }
            
        }

        public void OnSendFail()
        {
            failCount++;
        }

        public bool CanRetry()
        {
            // if (MessageType == nameof(DwhRetentionLog) || MessageType == nameof(DwhSessionLog)) return true;
            // return failCount < 2;

            return true;
        }
    }
}