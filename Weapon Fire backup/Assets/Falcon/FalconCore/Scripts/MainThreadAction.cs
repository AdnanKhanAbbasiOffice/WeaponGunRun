using System;
using System.Threading;
using Falcon.FalconCore.Scripts.Utils.FActions.Base;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts;

namespace Falcon.FalconCore.Scripts
{
    public class MainThreadAction : ChainAction
    {
        public MainThreadAction(IContinuableAction baseAction) : base(baseAction)
        {
        }

        public MainThreadAction(Action baseAction) : base(new UnitAction(baseAction))
        {
        }

        public override void Invoke()
        {
            if (Thread.CurrentThread.ManagedThreadId == FalconMain.MainThreadId)
            {
                base.Invoke();
            }
            else
            {
                FalconMain.Instance.OnUpdate += EventHandler;
            }
        }
        
        protected void EventHandler(object o, EventArgs eventArgs)
        {
            base.Invoke();
            FalconMain.Instance.OnUpdate -= EventHandler;
        }
    }
    
    public class MainThreadAction<T> : MainThreadAction, IChainAction<T>
    {
        public MainThreadAction(IContinuableAction<T> baseAction) : base(baseAction)
        {
        }

        public MainThreadAction(Func<T> baseAction) : base(new UnitAction<T>(baseAction))
        {
        }

        public bool TryInvoke(out T result)
        {
            if (FalconMain.MainThreadId.HasValue)
            {
                Invoke();
                while (!Done && Exception != null)
                {
                    Thread.Yield();
                }

                if (Exception != null) throw Exception;
                result = Result;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }

        public T Result => ((IContinuableAction<T>)BaseAction).Result;
    }
}