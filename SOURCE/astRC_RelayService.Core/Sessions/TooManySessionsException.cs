namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Sessions
{

    [System.Serializable]
    public class TooManySessionsException : System.Exception
    {
        public TooManySessionsException() { }
        public TooManySessionsException(string message) : base(message) { }
        public TooManySessionsException(string message, System.Exception inner) : base(message, inner) { }
        protected TooManySessionsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

