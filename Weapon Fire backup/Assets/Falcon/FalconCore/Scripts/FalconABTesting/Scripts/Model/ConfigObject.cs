using System;
using Newtonsoft.Json;

namespace Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model
{
    [Serializable]
    public class ConfigObject
    {
        [JsonProperty(PropertyName = "name")] 
        public readonly string Name;

        [JsonProperty(PropertyName = "value")] 
        public readonly object Value;

        [JsonConstructor]
        public ConfigObject(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public ConfigObject(ReceiveConfigObject receiveConfigObject)
        {
            Name = receiveConfigObject.Name;
            Value = receiveConfigObject.Value;
        }
    }
}