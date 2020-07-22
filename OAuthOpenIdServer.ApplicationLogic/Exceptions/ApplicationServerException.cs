using System;

namespace OAuthOpenIdServer.ApplicationLogic.Exceptions
{
    public class ApplicationServerException : Exception
    {
        public ApplicationServerException()
        {
        }

        public ApplicationServerException(string message) : base(message)
        {
        }

        public ApplicationServerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}