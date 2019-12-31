using System;
using System.Runtime.Serialization;

namespace Skender.Stock.Indicators
{
    // ref: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/exceptions/creating-and-throwing-exceptions#defining-exception-classes

    [Serializable()]
    public class BadHistoryException : Exception
    {
        public BadHistoryException() { }
        public BadHistoryException(string message) : base(message) { }
        public BadHistoryException(string message, Exception innerException) : base(message, innerException) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected BadHistoryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
