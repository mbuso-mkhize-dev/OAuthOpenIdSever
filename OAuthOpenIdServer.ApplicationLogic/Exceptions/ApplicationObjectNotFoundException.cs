using System;

namespace OAuthOpenIdServer.ApplicationLogic.Exceptions
{
    public class ApplicationObjectNotFoundException : Exception
    {
        public ApplicationObjectNotFoundException()
        {
        }

        public ApplicationObjectNotFoundException(string message) : base(message)
        {
        }

        public ApplicationObjectNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}