using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Falcon.FalconCore.Scripts.Utils.Sequences.Core;
using UnityEngine.Networking;

namespace Falcon.FalconCore.Scripts.Utils.Sequences.Entity
{
	public class HttpSequence : Sequence<String>
	{
		public HttpSequence( HttpMethod requestType, string url, string jsonBody = null,
			Dictionary<string, string> headers = null )
		{
			RequestType = requestType;
			URL = url;
			JsonBody = jsonBody;
			Headers = headers ?? new Dictionary<string, string>();
		}

		private HttpMethod RequestType { get; }
		private string URL { get; }
		private Dictionary<string, string> Headers { get; }
		private string JsonBody { get; }

		protected override IEnumerator<string> EnumeratorT()
		{
			using (UnityWebRequest webRequest = new UnityWebRequest(URL, RequestType.ToString()))
			{
				if( JsonBody != null && JsonBody.Trim().Length > 0 )
				{
					var jsonToSend = new UTF8Encoding().GetBytes( JsonBody );
					webRequest.uploadHandler = new UploadHandlerRaw( jsonToSend );
					Headers.Add( "Content-Type", "application/json" ); 
				}

				foreach (KeyValuePair<string, string> header in Headers)
				{
					webRequest.SetRequestHeader( header.Key, header.Value );
				}

				webRequest.downloadHandler = new DownloadHandlerBuffer();

				//Send the request then wait here until it returns
				webRequest.SendWebRequest();

				while( !webRequest.isDone )
				{
					yield return null;
				}

				while( !webRequest.downloadHandler.isDone )
				{
					yield return null;
				}

# if UNITY_2020_2_OR_NEWER
                if (webRequest.result == UnityWebRequest.Result.ConnectionError
                    || webRequest.result == UnityWebRequest.Result.ProtocolError
                    || webRequest.result == UnityWebRequest.Result.DataProcessingError)
                    throw new Exception(webRequest.error);
#else
				if( webRequest.isHttpError || webRequest.isNetworkError)
				{
					throw new SequenceException( webRequest.error );
				}
#endif
				yield return webRequest.downloadHandler.text;
			}
		}
	}
}