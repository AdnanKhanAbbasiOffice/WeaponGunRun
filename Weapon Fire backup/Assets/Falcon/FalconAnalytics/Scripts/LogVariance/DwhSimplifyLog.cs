using System;

namespace Falcon.FalconAnalytics.Scripts.LogVariance
{
    [Obsolete("DwhLogCache is now fused into DWHLog, there is now no reason to use DwhLogCache anymore.")]
    public static class DwhSimplifyLog 
    {
        public static ILog Instance => DWHLog.Instance;
    }
}