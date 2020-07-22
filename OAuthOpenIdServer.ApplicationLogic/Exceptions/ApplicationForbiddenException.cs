using System;

namespace OAuthOpenIdServer.ApplicationLogic.Exceptions
{
    public class ApplicationForbiddenException : Exception
    {
        public ApplicationForbiddenException()
        {
        }

        public ApplicationForbiddenException(string message) : base(message)
        {
        }

        public ApplicationForbiddenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}