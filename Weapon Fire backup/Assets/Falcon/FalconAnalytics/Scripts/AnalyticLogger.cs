using Falcon.FalconCore.Scripts.Utils.Logs;

namespace Falcon.FalconAnalytics.Scripts
{
    public class AnalyticLogger : FalconLog<AnalyticLogger>
    {
        protected override string GetColor()
        {
            return "#35adcf";
        }
    }
}

