using Newtonsoft.Json;

namespace Falcon.FalconCore.Editor.FPlugins.BitBucketObjs
{
    public class BitBucObj
    {
        [JsonProperty(PropertyName = "path")]public string Path;
        [JsonProperty(PropertyName = "links")]public BitBucLink Links;
    }

    public class BitBucLink
    {
        [JsonProperty(PropertyName = "self")] public BitBucRef Self; 
        [JsonProperty(PropertyName = "meta")]public BitBucRef Meta;
        [JsonProperty(PropertyName = "history")]public BitBucRef History;
    }

    public class BitBucRef
    {
        [JsonProperty(PropertyName = "href")]public string HRef;
    }
}