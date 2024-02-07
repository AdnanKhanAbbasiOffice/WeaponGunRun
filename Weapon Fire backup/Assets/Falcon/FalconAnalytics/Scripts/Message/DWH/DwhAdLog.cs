using System;
using Falcon.FalconAnalytics.Scripts.Enum;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhAdLog : DwhMessage
    {
        public string type;

        public string adWhere;

        public string adPrecision;
        public string adCountry;
        public double adRev;
        public string adNetwork;
        public string adMediation;
        
        public DwhAdLog(AdType type, string adWhere, long clientCreateDate) : base(clientCreateDate)
        {
            this.type = type.ToString();
            this.adWhere =  CheckStringLength(adWhere, nameof(adWhere),50);
        }

        [Preserve]
        [JsonConstructor]
        public DwhAdLog(AdType type, string adWhere, string adPrecision, string adCountry, double adRev, string adNetwork, string adMediation, long clientCreateDate) : base(clientCreateDate)
        {
            this.type = type.ToString();
            this.adWhere = CheckStringLength(adWhere, nameof(adWhere),50);
            this.adPrecision = CheckStringLength(adPrecision,nameof(adPrecision),20);
            this.adCountry = CheckStringLength(adCountry, nameof(adCountry),2);
            this.adRev = CheckNumberNonNegative(adRev, nameof(adRev));
            this.adNetwork = CheckStringLength(adNetwork, nameof(adNetwork),20);
            this.adMediation = CheckStringLength(adMediation, nameof(adMediation),20);
        }

        protected override string GetAPI()
        {
            return DwhConstants.AdApi;
        }

    }
}

