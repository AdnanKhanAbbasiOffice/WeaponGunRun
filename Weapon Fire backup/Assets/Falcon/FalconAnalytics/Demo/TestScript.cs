using System;
using Falcon.FalconAnalytics.Scripts.Enum;
using UnityEngine;

namespace Falcon.FalconAnalytics.Demo
{
    public class TestScript : MonoBehaviour
    {
        public void BtnResourceLog()
        {
            DWHLog.Log.ResourceLog(1, FlowType.Source, "campaign_dropingame", "startgame", "gold", 186);
        }

        public void BtnInappLog()
        {
            DWHLog.Log.InAppLog(1,"inapp_gem_50usd", "KRW", "₩60,000", "GPA.3306-6137-6537-83141", "cfgajadoapoppjolpjcccbhj.AO-J1OwAwGuQ3hemNLsIqtaT3UQmD31i7Yk9sR4by6QYgWSrtC_Xg8fw5t1OR8w5jBtpTyiGiZU_VdKi-La2yAD3d2FrsHN9XA", "Purchased");

        }
        public void BtnLevelLog()
        {
            DWHLog.Log.LevelLog(1, TimeSpan.FromSeconds(154), 3, "normal", LevelStatus.Fail);
        }
        public void BtnAdsLog()
        {
            DWHLog.Log.AdsLog(1,AdType.Reward, "Revive_Player");

        }
    }
}
