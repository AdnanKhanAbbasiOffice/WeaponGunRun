namespace Falcon.FalconCore.Scripts.Utils.FActions.Base
{
    public abstract class StartAction : FAction, IStartAction
    {
        public override bool CanInvoke()
        {
            return true;
        }
    }
}