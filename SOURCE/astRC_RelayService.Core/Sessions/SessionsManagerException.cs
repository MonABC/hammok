using System;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Sessions
{

    [Serializable]
    public class SessionsManagerException : Exception
    {
        public SessionsManagerException() { }
        public SessionsManagerException(string message) : base(message) { }
        public SessionsManagerException(string message, Exception inner) : base(message, inner) { }
        protected SessionsManagerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
