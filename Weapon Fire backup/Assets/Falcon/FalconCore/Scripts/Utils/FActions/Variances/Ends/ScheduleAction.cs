using System;
using Falcon.FalconCore.Scripts.Utils.FActions.Base;
using Falcon.FalconCore.Scripts.Utils.FActions.Variances.Starts;

namespace Falcon.FalconCore.Scripts.Utils.FActions.Variances.Ends
{
    public class ScheduleAction : EndAction
    {
        private long invokableTime;
        private bool cancel;

        public TimeSpan TimeSpan { get; set; }

        public ScheduleAction(IContinuableAction baseAction, TimeSpan timeSpan) : base(baseAction)
        {
            this.TimeSpan = timeSpan;
            invokableTime = FTime.CurrentTimeMillis();
        }
        
        public ScheduleAction(Action action, TimeSpan timeSpan) : this(new UnitAction(action), timeSpan)
        {
        }

        public override void Invoke() {
            if (cancel) {
                return;
            }
            invokableTime = FTime.CurrentTimeMillis() + (long) TimeSpan.TotalMilliseconds;
            Schedule();
            base.Invoke();
        }

        public override bool CanInvoke()
        {
            if (invokableTime > FTime.CurrentTimeMillis())
            {
                return false;
            }

            return base.CanInvoke();
        }

        public override void Cancel()
        {
            cancel = true;
            base.Cancel();
        }
    }
}

