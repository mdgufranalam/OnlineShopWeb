﻿using System.Runtime.Serialization;

namespace OnlineShop.ServiceHelper
{
    [Serializable]
    public class ConnectionException : Exception
    {
        public ConnectionException()
        {
        }

        public ConnectionException(string? message) : base(message)
        {
        }

        public ConnectionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}