using System;
using System.Threading;
using Falcon.FalconAnalytics.Scripts.Counters;
using Falcon.FalconAnalytics.Scripts.Message.Wrapper;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Data;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhActionLog : DwhMessage
    {
        private static readonly string SessionGuid = Guid.NewGuid().ToString();
        
        public string aFrom;
        public int priority;
        public int sessionId;
        public int aTime;
        public string aTo;
        public string sessionUid = SessionGuid;

        static DwhActionLog()
        {
            FalconMain.Instance.OnGameContinue += (sender, args) =>
                MessageSender.Instance.Enqueue(new DwhActionLog("GameContinue", FTime.CurrentTimeMillis()));
            FalconMain.Instance.OnGameStop += (sender, args) =>
            {
                DataWrapper dataWrapper = new DwhActionLog("GameStop", FTime.CurrentTimeMillis()).Wrap();
                new Thread(() =>
                {
                    try
                    {
                        dataWrapper.Send();
                    }
                    catch (Exception e)
                    {
                        AnalyticLogger.Instance.Warning(e.Message);
                    }
                }).Start();
            };
        }
        
        public DwhActionLog(string actionName, long clientCreateDate) : base(clientCreateDate)
        {
            if (LastActionTime == -1) LastActionTime = PlayTimeCounter.SessionStartTime;
            aFrom = LastAction;
            aTo = CheckStringLength(actionName, nameof(actionName),50);
            aTime = (int)(FTime.CurrentTimeSec() - LastActionTime);
            sessionId = PlayerParams.SessionId;

            LastActionTime = FTime.CurrentTimeMillis();
            LastAction = aTo;
        }

        [Preserve]
        [JsonConstructor]
        public DwhActionLog(string aFrom, int priority, int sessionId, int aTime, string aTo, long clientCreateDate) : base(clientCreateDate)
        {
            this.aFrom = aFrom;
            this.priority = priority;
            this.sessionId = sessionId;
            this.aTime = aTime;
            this.aTo = aTo;
        }

        private static string LastAction { get; set; } = "GameStart";
        private static long LastActionTime { get; set; } = -1;

        protected override string GetAPI()
        {
            return DwhConstants.ActionApi;
        }
    }
}