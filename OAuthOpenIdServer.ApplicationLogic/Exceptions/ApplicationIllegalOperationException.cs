using System;

namespace OAuthOpenIdServer.ApplicationLogic.Exceptions
{
    public class ApplicationIllegalOperationException : Exception
    {
        public ApplicationIllegalOperationException()
        {
        }

        public ApplicationIllegalOperationException(string message) : base(message)
        {
        }

        public ApplicationIllegalOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}