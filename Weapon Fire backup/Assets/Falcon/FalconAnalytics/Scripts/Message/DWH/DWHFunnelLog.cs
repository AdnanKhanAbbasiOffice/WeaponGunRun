using System;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Data;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhFunnelLog : DwhMessage
    {
        public string action;
        public string funnelName;
        public int priority;
        public string funnelDay;

        [Preserve]
        [JsonConstructor]
        public DwhFunnelLog(string funnelName, string action, int priority, long clientCreateDate) : base(clientCreateDate)
        {
            this.action = CheckStringLength(action,nameof(action),200);
            this.funnelName = CheckStringLength(funnelName, nameof(funnelName), 200);
            this.priority = CheckNumberNonNegative(priority, nameof(priority));
            
            if (priority != 0 && !FData.Instance.HasKey(this.funnelName + (priority - 1)))
            {
                AnalyticLogger.Instance.Error($"Dwh Log invalid logic : Funnel {this.funnelName} not created in order in this device instance");
            }

            FData.Instance.Compute<DateTime>(this.funnelName + priority, (hasKey, val) =>
            {
                if (hasKey)
                {
                    AnalyticLogger.Instance.Error(
                        $"Dwh Log invalid logic : This device already joined the funnel {this.funnelName} of the priority {priority}");
                }

                return DateTime.Now.ToUniversalTime();
            });

            DateTime day = FData.Instance.GetOrSet(this.funnelName + 0, DateTime.Today.ToUniversalTime()).ToLocalTime();
            funnelDay = FTime.DateToString(day);
        }

        protected override string GetAPI()
        {
            return DwhConstants.FunnelApi;
        }
    }
}