using System;

namespace Falcon.FalconCore.Scripts.Utils.FActions.Base
{
    public interface IFAction
    {
        Exception Exception { get;}
        bool Done { get; }

        /// <summary>
        ///     Call this schedule and the function will arrange itself to be executed,
        /// </summary>
        void Schedule();

        void Cancel();

        void Invoke();
        
        bool CanInvoke();
    }

    public interface IContinuableAction : IFAction
    {
    }

    public interface IFollowAction : IFAction
    {
    }

    public interface IStartAction : IContinuableAction
    {
    }

    public interface IChainAction : IContinuableAction, IFollowAction
    {
    }

    public interface IEndAction : IFollowAction
    {
    }

    public interface IFAction<out T> : IFAction
    {
        T Result { get; }
    }

    public interface IContinuableAction<out T> : IFAction<T>, IContinuableAction
    {
    }

    public interface IFollowAction<out T> : IFAction<T>, IFollowAction
    {
    }

    public interface IStartAction<out T> : IContinuableAction<T>, IStartAction
    {
        /// <summary>
        /// Wait till CanInvoke(), then Invoke() and return Result;
        /// </summary>
        /// <returns></returns>
        T InvokeAndGet();
    }

    public interface IChainAction<T> : IContinuableAction<T>, IChainAction, IFollowAction<T>
    {
        /// <summary>
        /// Try to Invoke() to get the action immediately
        /// </summary>
        /// <param name="result"></param>
        /// <returns>whether the function invoked</returns>
        bool TryInvoke(out T result);
    }
}