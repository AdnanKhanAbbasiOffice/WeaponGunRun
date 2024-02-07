using System.Collections.ObjectModel;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Payload
{
    
    public class ReceiveConfig
    {
        [JsonProperty(PropertyName = "runningABTesting")]
        public readonly string RunningAbTesting;
        
        [JsonProperty(PropertyName = "configs")]
        public readonly ReadOnlyCollection<ReceiveConfigObject> Configs;

        [Preserve]
        [JsonConstructor]
        public ReceiveConfig(string runningAbTesting, ReceiveConfigObject[] configs)
        {
            RunningAbTesting = runningAbTesting;
            Configs = new ReadOnlyCollection<ReceiveConfigObject>(configs);
            
        }
    }
}