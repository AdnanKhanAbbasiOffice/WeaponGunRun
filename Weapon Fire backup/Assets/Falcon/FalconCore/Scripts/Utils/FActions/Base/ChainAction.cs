using System;

namespace Falcon.FalconCore.Scripts.Utils.FActions.Base
{
    public abstract class ChainAction : FAction, IChainAction
    {
        protected readonly IContinuableAction BaseAction;

        protected ChainAction(IContinuableAction baseAction)
        {
            BaseAction = baseAction;
        }

        public override Exception Exception => BaseAction.Exception;
        public override bool Done => BaseAction.Done;

        public override void Invoke()
        {
            BaseAction.Invoke();
        }

        public override bool CanInvoke()
        {
            return BaseAction.CanInvoke();
        }

        public override void Cancel()
        {
            BaseAction.Cancel();
        }
    }
}