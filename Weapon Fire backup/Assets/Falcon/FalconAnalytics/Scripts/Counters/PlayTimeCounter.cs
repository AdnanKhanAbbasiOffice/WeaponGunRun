using System.Collections;
using System.Threading;
using Falcon.FalconAnalytics.Scripts.Message.DWH;
using Falcon.FalconAnalytics.Scripts.Message.Wrapper;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.Interfaces;
using Falcon.FalconCore.Scripts.Utils;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Counters
{
    public sealed class PlayTimeCounter : IFInit
    {
        public static long SessionStartTime { get; private set; }
        private const string TotalTimeKey = "USER_TOTAL_TIME";
        private long startTime;

        [Preserve]
        public PlayTimeCounter()
        {
        }

        public IEnumerator Init()
        {
            startTime = FTime.CurrentTimeSec();
            SessionStartTime = startTime;
            FalconMain.Instance.OnGameStop += (a, b) => SaveTotalTime();
            FalconMain.Instance.OnGameContinue += (a, b) => startTime = FTime.CurrentTimeSec();
            yield return null;
        }

        private void SaveTotalTime()
        {
            DataWrapper dataWrapper = new DwhSessionLog((int)(FTime.CurrentTimeSec() - startTime), TotalTimeKey, FTime.CurrentTimeMillis())
                .Wrap();
            new Thread(() =>
            {
                try
                {
                    dataWrapper.Send();
                }
                catch (System.Exception e)
                {
                    AnalyticLogger.Instance.Warning(e.Message);
                }
            }).Start();
        
            AnalyticLogger.Instance.Info("Current session time : " + (FTime.CurrentTimeSec() - startTime));
        }
    }
}