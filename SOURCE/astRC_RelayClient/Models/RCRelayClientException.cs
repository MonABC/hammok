using System;
using System.Runtime.Serialization;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models
{
    [Serializable]
    public class RCRelayClientException : Exception
    {
        [Obsolete("この例外を使用する場合、メッセージを入力してください。", true)]
        public RCRelayClientException()
        {
        }

        public RCRelayClientException(string message) : base(message)
        {
        }

        public RCRelayClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RCRelayClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}