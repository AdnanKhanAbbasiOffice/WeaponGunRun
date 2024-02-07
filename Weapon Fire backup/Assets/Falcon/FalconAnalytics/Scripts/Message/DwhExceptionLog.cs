using System;
using System.Net.Http;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts;
using Newtonsoft.Json;

namespace Falcon.FalconAnalytics.Scripts.Message
{
    public class DwhExceptionLog
    {
        [JsonProperty(PropertyName = "message")]
        public readonly string Message;

        public DwhExceptionLog(object data)
        {
            Message = data.ToString();
        }

        public void Send()
        {
            new HttpRequest
            {
                RequestType = HttpMethod.Post,
                URL = DwhConstants.ExceptionApi,
                JsonBody = JsonConvert.SerializeObject(this),
                Timeout = TimeSpan.FromSeconds(5)
            }.Schedule();
        }
    }
}