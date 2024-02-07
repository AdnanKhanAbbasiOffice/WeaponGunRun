using Falcon.FalconCore.Scripts.Exceptions;

namespace Falcon.FalconCore.Scripts.Utils.Sequences.Core
{
    public class SequenceException : FSdkException
    {
        public SequenceException(string message) : base(message)
        {
        }
    }
}