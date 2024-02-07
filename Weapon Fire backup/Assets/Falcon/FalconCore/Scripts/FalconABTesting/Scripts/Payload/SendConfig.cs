using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using Falcon.FalconCore.Scripts.Utils.Data;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts;
using Falcon.FalconCore.Scripts.Utils.Logs;
using Newtonsoft.Json;

namespace Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Payload
{
    public class SendConfig
    {
#pragma warning disable S1075 // URIs should not be hardcoded    
        [JsonIgnore] private const string ServerURL = "https://gateway.data4game.com/kapigateway/abtestingservice/rmconfigs/getRemoteConfigsByFilters";
#pragma warning restore S1075 // URIs should not be hardcoded
        
        [JsonProperty(PropertyName = "NumberOfVideo")]
        public readonly int NumberOfVideo = PlayerParams.InterstitialAdCount + PlayerParams.RewardAdCount;

        [JsonProperty(PropertyName = "Level")] 
        public readonly int Level = PlayerParams.MaxPassedLevel;

        [JsonProperty(PropertyName = "createdDate")]
        public readonly long CreatedDate = FDeviceInfo.FirstLogInMillis / 1000;//millis second -> second

        [JsonProperty(PropertyName = "runningABTesting")]
        public readonly string RunningAbTesting = FalconConfig.RunningAbTesting;

        [JsonProperty(PropertyName = "properties")]
        public readonly ReadOnlyCollection<ConfigObject> Properties;

        [JsonProperty(PropertyName = "abTestingConfigs")]
        public readonly ReadOnlyCollection<ConfigObject> AbTestingConfigs =
            new ReadOnlyCollection<ConfigObject>(FalconConfig.TestingConfigs != null
                ? FalconConfig.TestingConfigs
                : new List<ConfigObject>());
        
        public SendConfig()
        {
            Properties = new ReadOnlyCollection<ConfigObject>(new List<ConfigObject>
            {
                new ConfigObject("deviceName", FDeviceInfo.DeviceName),
                new ConfigObject("numberOfVideos", NumberOfVideo.ToString()),
                new ConfigObject("platform", FDeviceInfo.Platform),
                new ConfigObject("appversion",FDeviceInfo.AppVersion),
                new ConfigObject("level", Level.ToString())
            });
        }

        public ReceiveConfig Connect()
        {
            CoreLogger.Instance.Info(JsonConvert.SerializeObject(this));
            string response = new HttpRequest
            {
                RequestType = HttpMethod.Post,
                URL = ServerURL,
                JsonBody = JsonConvert.SerializeObject(this),
                Headers = new Dictionary<string, string> { { "Game-Id", FDeviceInfo.GameId } }
            }.InvokeAndGet();
            
            CoreLogger.Instance.Info(response);
            return JsonConvert.DeserializeObject<ReceiveConfig>(response);
        }
    }
}