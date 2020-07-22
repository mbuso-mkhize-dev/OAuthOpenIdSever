using System;

namespace OAuthOpenIdServer.ApplicationLogic.Exceptions
{
    public class ApplicationDuplicateDataException : Exception
    {
        public ApplicationDuplicateDataException()
        {
        }

        public ApplicationDuplicateDataException(string message) : base(message)
        {
        }

        public ApplicationDuplicateDataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}