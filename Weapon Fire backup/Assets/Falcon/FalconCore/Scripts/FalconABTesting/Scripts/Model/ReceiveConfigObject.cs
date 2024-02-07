using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model
{
    public class ReceiveConfigObject
    {
        [JsonProperty(PropertyName = "name")] public string Name;

        [JsonProperty(PropertyName = "value")] public object Value;

        [JsonProperty(PropertyName = "abTesting")] public bool AbTesting;
        
        [Preserve]
        [JsonConstructor]
        public ReceiveConfigObject(string name, object value, bool abTesting)
        {
            Name = name;
            Value = value;
            AbTesting = abTesting;
        }
    }
}