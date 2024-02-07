using System.Collections.Generic;
using System.Net.Http;
using Falcon.FalconCore.Scripts.Utils.Sequences.Core;
using Falcon.FalconCore.Scripts.Utils.Sequences.Entity;
using Newtonsoft.Json;

namespace Falcon.FalconCore.Editor.FPlugins.BitBucketObjs
{
    public class BitBucCall: Sequence<List<BitBucObj>>
    {
        private readonly string url;

        public BitBucCall(string url)
        {
            this.url = url;
        }

        protected override IEnumerator<List<BitBucObj>> EnumeratorT()
        {
            List<BitBucObj> result = new List<BitBucObj>();
            
            var currentUrl = url;

            while (!string.IsNullOrEmpty(currentUrl))
            {
                var httpSequence = new HttpSequence(HttpMethod.Get, currentUrl);

                while (httpSequence.MoveNext()) yield return null;

                BitBucResponse response = JsonConvert.DeserializeObject<BitBucResponse>(httpSequence.Current);
                result.AddRange(response.Values);

                currentUrl = response.Next;
            }

            yield return result;
        }
    }
}