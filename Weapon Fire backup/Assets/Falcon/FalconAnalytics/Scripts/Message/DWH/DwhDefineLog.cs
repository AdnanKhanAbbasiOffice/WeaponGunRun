using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhDefineLog : DwhMessage
    {
        public string type;
        public string jsonContent;
        
        [Preserve]
        [JsonConstructor]
        public DwhDefineLog(string type, string jsonContent, long clientCreateDate) : base(clientCreateDate)
        {
            this.type = CheckStringLength(type, nameof(type), 30);
            this.jsonContent = CheckStringLength(jsonContent, nameof(jsonContent), 2000);
        }


        protected override string GetAPI()
        {
            return DwhConstants.DefineLogApi;
        }

    }
}