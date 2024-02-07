using System;
using Falcon.FalconAnalytics.Scripts.Enum;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhResourceLog : DwhMessage
    {
        public long amount;
        public string currency;
        public string flowType;
        public string itemId;
        public string itemType;

        public DwhResourceLog(FlowType flowType, string itemType, string itemId, string currency, long amount, long clientCreateDate) : base(clientCreateDate)
        {
            this.flowType = flowType.ToString();
            this.itemType = CheckStringLength(itemType, nameof(itemType),50);
            this.itemId = CheckStringLength(itemId, nameof(itemId),50);
            this.currency = CheckStringLength(currency,nameof(currency),50);
            this.amount = CheckNumberNonNegative(amount, nameof(amount));
        }

        [Preserve]
        [JsonConstructor]
        public DwhResourceLog(string flowType, string itemType, string itemId, string currency, long amount, long clientCreateDate) : this(
            (FlowType)System.Enum.Parse(typeof(FlowType), flowType), itemType, itemId, currency, amount, clientCreateDate)
        {
        }

        protected override string GetAPI()
        {
            return DwhConstants.ResourceApi;
        }
    }
}