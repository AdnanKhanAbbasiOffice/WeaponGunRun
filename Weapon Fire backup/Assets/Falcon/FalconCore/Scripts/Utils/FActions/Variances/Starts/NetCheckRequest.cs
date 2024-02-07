using System;
using System.Globalization;
using System.Net;
using Falcon.FalconCore.Scripts.Utils.FActions.Base;

namespace Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts
{
    public class NetCheckRequest : StartAction, IStartAction<bool>
    {
        private Exception exception;
        private bool isDone;

        public int TimeoutMs { get; set; } = 10000;

        public bool InvokeAndGet()
        {
            Invoke();
            return Result;
        }

        public override Exception Exception => exception;
        public override bool Done => isDone;

        public override void Invoke()
        {
            try
            {
                string url;

                if (CultureInfo.InstalledUICulture.Name.StartsWith("fa"))
                    url = "https://www.aparat.com";
                else if (CultureInfo.InstalledUICulture.Name.StartsWith("zh"))
                    url = "https://www.baidu.com";
                else
                    url = "https://www.google.com";

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = TimeoutMs;
                using (request.GetResponse())
                {
                    Result = true;
                }
            }
            catch (Exception e)
            {
                Result = false;
                exception = e;
            }

            isDone = true;
        }

        public bool Result { get; private set; }
    }
}