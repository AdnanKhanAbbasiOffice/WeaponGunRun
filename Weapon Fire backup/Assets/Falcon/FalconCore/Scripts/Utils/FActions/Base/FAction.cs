using System;
using System.Collections;
using System.Threading;
using Falcon.FalconCore.Scripts.Entities;
using UnityEngine;

namespace Falcon.FalconCore.Scripts.Utils.FActions.Base
{
    public abstract class FAction : IFAction
    {
        public abstract Exception Exception { get;}
        public abstract bool Done { get; }
        
        public void Schedule()
        {
            ActionQueue.Enqueue(this);
        }

        public virtual void Cancel()
        {
            ActionQueue.Remove(this);
        }

        public abstract void Invoke();
        
        public abstract bool CanInvoke();

        #region Scheduling

        private static readonly FQueue<FAction> ActionQueue = new FQueue<FAction>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitFAction()
        {
            FalconMain.Instance.StartCoroutine(CheckLoop());
        }

        private static IEnumerator CheckLoop()
        {
            while (FalconMain.ApplicationRunning)
            {
                var actions = ActionQueue.Clear();
                foreach (var action in actions)
                    if (action.CanInvoke())
                        ThreadPool.QueueUserWorkItem(_ => { action.Invoke(); });
                    else
                        ActionQueue.Enqueue(action);

                yield return new WaitForSeconds(1);
            }
        }

        #endregion
    }
}