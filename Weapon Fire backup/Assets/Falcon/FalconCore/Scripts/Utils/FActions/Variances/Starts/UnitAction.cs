using System;
using Falcon.FalconCore.Scripts.Utils.FActions.Base;

namespace Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts
{
    public class UnitAction : StartAction, IStartAction
    {
        private readonly Action action;
        private Exception exception;
        protected bool isDone;

        public UnitAction(Action action)
        {
            this.action = action;
        }

        public override Exception Exception => exception;
        public override bool Done => isDone;

        public override void Invoke()
        {
            try
            {
                action.Invoke();
                isDone = true;
            }
            catch (Exception e)
            {
                exception = e;
            }
        }
    }

    public class UnitAction<T> : UnitAction, IStartAction<T>
    {
        private readonly Func<T> action;

        public UnitAction(Func<T> action) : base(null)
        {
            this.action = action;
        }

        public override void Invoke()
        {
            Result = action.Invoke();
            isDone = true;
        }

        public T Result { get; private set; }
        
        public T InvokeAndGet()
        {
            Invoke();
            return Result;
        }
    }
}