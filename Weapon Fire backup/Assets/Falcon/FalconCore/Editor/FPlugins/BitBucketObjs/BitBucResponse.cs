using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconCore.Editor.FPlugins.BitBucketObjs
{
    public class BitBucResponse
    {
        [JsonProperty(PropertyName = "values")] public List<BitBucObj> Values;
        [JsonProperty(PropertyName = "pagelen")] public int PageLength;
        [JsonProperty(PropertyName = "page")] public int Page;
        [JsonProperty(PropertyName = "next")] public string Next;

        [Preserve]
        public BitBucResponse()
        {
        }
        
        
    }
}