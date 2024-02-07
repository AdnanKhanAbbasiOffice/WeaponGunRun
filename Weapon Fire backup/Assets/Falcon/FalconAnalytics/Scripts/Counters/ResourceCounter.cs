using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Falcon.FalconAnalytics.Scripts.Enum;
using Falcon.FalconAnalytics.Scripts.Message.DWH;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Ends;
using Falcon.FalconCore.Scripts.Utils.Singletons;

namespace Falcon.FalconAnalytics.Scripts.Counters
{
    public class ResourceCounter : FSingleton<ResourceCounter>
    {
        private readonly ConcurrentDictionary<ResourceInfo, long> countMap =
            new ConcurrentDictionary<ResourceInfo, long>();

        public ResourceCounter()
        {
            FalconMain.Instance.OnGameStop += (a, b) => FlushCountMap();
            new ScheduleAction(new WaitInit(FlushCountMap), TimeSpan.FromSeconds(5)).Schedule();
        }

        public void Summarize(FlowType flowType, string itemType, string itemId, string currency,
            long amount)
        {
            var info = new ResourceInfo(flowType, itemType, itemId, currency);
            countMap.AddOrUpdate(info, amount, (_, oldValue) => oldValue + amount);
        }

        private void FlushCountMap()
        {
            long now = FTime.CurrentTimeMillis();
            var infos = new List<ResourceInfo>(countMap.Keys);

            foreach (var info in infos)
            {
                long sumValue;
                if (countMap.TryRemove(info, out sumValue))
                    MessageSender.Instance.Enqueue(new DwhResourceLog(info.FlowType, info.ItemType, info.ItemId, info.Currency,
                        sumValue, now));
            }
        }

        private struct ResourceInfo
        {
            public readonly FlowType FlowType;
            public readonly string ItemType;
            public readonly string ItemId;
            public readonly string Currency;

            public ResourceInfo(FlowType flowType, string itemType, string itemId, string currency)
            {
                if (itemType == null) itemType = "";

                if (itemId == null) itemId = "";

                if (currency == null) currency = "";

                FlowType = flowType;
                ItemType = itemType;
                ItemId = itemId;
                Currency = currency;
            }

            /**
             * Keep this method accessor protected or it will not work ._.
             */
            public bool Equals(ResourceInfo other)
            {
                return FlowType == other.FlowType &&
                       ItemType == other.ItemType && ItemId == other.ItemId && Currency == other.Currency;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (obj.GetType() != GetType()) return false;
                return Equals((ResourceInfo)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (int)FlowType;
                    hashCode = (hashCode * 397) ^ (ItemType != null ? ItemType.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (ItemId != null ? ItemId.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Currency != null ? Currency.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
}