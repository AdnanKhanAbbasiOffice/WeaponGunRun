using Falcon.FalconCore.Scripts.Exceptions;

namespace Falcon.FalconAnalytics.Scripts.Message.Exceptions
{
    public class DwhMessageException : FSdkException
    {
        public DwhMessageException(string message) : base(message)
        {
            
        }
    }
}