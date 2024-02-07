using System;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;

namespace Falcon.FalconAnalytics.Scripts.Message.Wrapper
{
    [Serializable]
    public class ActionLogConfig : FalconConfig
    {
        public bool fCoreAnalyticActionLogEnable;
    }
}