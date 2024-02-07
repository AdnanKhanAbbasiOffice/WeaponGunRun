using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhPropertyLog : DwhMessage
    {
        public string pName;
        public string pValue;
        public int priority;

        [Preserve]
        [JsonConstructor]
        public DwhPropertyLog(string pName, string pValue, int priority, long clientCreateDate) : base(clientCreateDate)
        {
            this.pName = CheckStringLength(pName, nameof(pName),50);
            this.pValue = CheckStringLength(pValue,nameof(pValue), 50);
            this.priority = priority;
        }

        protected override string GetAPI()
        {
            return DwhConstants.PropertyApi;
        }
    }
}