using System;
using Falcon.FalconCore.Scripts.Utils.FActions.Base;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts;

namespace Falcon.FalconCore.Scripts.Utils.FActions.Variances.Chains
{
    public class DelayAction : ChainAction
    {
        private readonly long createTime = FTime.CurrentTimeMillis();
        private readonly TimeSpan delayTime;

        public DelayAction(IContinuableAction baseAction, TimeSpan delayTime) : base(baseAction)
        {
            this.delayTime = delayTime;
        }

        public DelayAction(Action action, TimeSpan delayTime) : this(new UnitAction(action), delayTime)
        {
        }

        public override bool CanInvoke()
        {
            if (createTime + delayTime.TotalMilliseconds > FTime.CurrentTimeMillis()) return false;
            return base.CanInvoke();
        }
    }

    public class DelayAction<T> : DelayAction, IChainAction<T>
    {

        public DelayAction(IContinuableAction<T> baseAction, TimeSpan delayTime) : base(baseAction, delayTime)
        {
        }

        public DelayAction(Func<T> action, TimeSpan delayTime) : this(new UnitAction<T>(action), delayTime)
        {
        }
        
        public T Result => ((IContinuableAction<T>)BaseAction).Result;
        
        public bool TryInvoke(out T result)
        {
            if (CanInvoke())
            {
                Invoke();
                result = Result;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }
    }
}