namespace Falcon.FalconCore.Scripts.Utils.Sequences.Core
{
    public class SequenceCancelException : SequenceException
    {
        public SequenceCancelException(string message) : base(message)
        {
        }
        
        public SequenceCancelException() : this("Sequence stopped on request")
        {
        }
    }
}