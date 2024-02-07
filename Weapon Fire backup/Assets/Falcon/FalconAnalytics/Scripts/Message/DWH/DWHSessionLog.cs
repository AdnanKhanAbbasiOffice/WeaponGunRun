using System;
using Falcon.FalconCore.Scripts.Utils.Data;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhSessionLog : DwhMessage
    {
        public string gameMode;
        public int sessionId;
        public int sessionTime;

        public DwhSessionLog(int sessionTime, string gameMode, long clientCreateDate) : base(clientCreateDate)
        {
            this.sessionTime = sessionTime;
            this.gameMode = CheckStringLength(gameMode, nameof(gameMode), 100);
            sessionId = PlayerParams.SessionId;
        }

        [Preserve]
        [JsonConstructor]
        public DwhSessionLog(string gameMode, int sessionId, int sessionTime, long clientCreateDate) : this(sessionId, gameMode, clientCreateDate)
        {
            this.sessionId = sessionId;
        }

        protected override string GetAPI()
        {
            return DwhConstants.SessionApi;
        }
    }
}