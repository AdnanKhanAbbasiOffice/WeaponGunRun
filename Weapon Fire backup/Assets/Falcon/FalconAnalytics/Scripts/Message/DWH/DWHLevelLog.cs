using System;
using Falcon.FalconAnalytics.Scripts.Enum;
using Falcon.FalconAnalytics.Scripts.Message.Exceptions;
using Falcon.FalconCore.Scripts.Utils;
using Falcon.FalconCore.Scripts.Utils.Data;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Falcon.FalconAnalytics.Scripts.Message.DWH
{
    [Serializable]
    public class DwhLevelLog : DwhMessage
    {

        public new int level;
        /// <summary>
        /// in second
        /// </summary>
        public int duration;
        public int wave;
        public string difficulty;
        public string status;
        
        [Preserve]
        [JsonConstructor]
        public DwhLevelLog(int level, int duration, int wave, string difficulty, LevelStatus status, long clientCreateDate) : base(clientCreateDate)
        {
            var levelKey = "HasPassedLevel_" + level + "_Difficulty_" + difficulty;

            FData.Instance.Compute<long?>(levelKey, (hasKey, val) =>
            {
                if (hasKey)
                {
                    AnalyticLogger.Instance.Error("Falcon Log detect error : Already passed the level before");
                }
                if (LevelStatus.Pass.Equals(status))
                {
                    PlayerParams.MaxPassedLevel = level;
                    return  FTime.CurrentTimeSec();
                }

                return null;
            });
            
            this.level = CheckNumberNonNegative(level, nameof(level));
            this.duration = CheckNumberNonNegative(duration, nameof(duration));
            this.wave = CheckNumberNonNegative(wave, nameof(wave));
            this.difficulty = CheckStringLength(difficulty, nameof(difficulty), 20);
            this.status = status.ToString();
        }
        
        
        protected override string GetAPI()
        {
            return DwhConstants.LevelApi;
        }
        

    }
}

