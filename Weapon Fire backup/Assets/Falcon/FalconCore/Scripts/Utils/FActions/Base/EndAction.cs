using System;

namespace Falcon.FalconCore.Scripts.Utils.FActions.Base
{
    public abstract class EndAction : FAction, IEndAction
    {
        protected readonly IContinuableAction BaseAction;

        protected EndAction(IContinuableAction baseAction)
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
    }
}