namespace Falcon.FalconCore.Scripts.Utils
{
    public enum ExecState
    {
        NotStarted = 0,
        Processing = 1,
        Succeed = 2,
        Failed = 3
    }

    public static class ExecStates
    {
        public static bool CanStart(ExecState state)
        {
            return state == ExecState.Failed || state == ExecState.NotStarted;
        }
    }
}