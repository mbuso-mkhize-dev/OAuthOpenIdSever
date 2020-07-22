using System;

namespace OAuthOpenIdServer.Models
{
    [Serializable]
    public class ApiErrorModel
    {
        public ApiErrorModel()
        {
        }

        public ApiErrorModel(
            string error,
            string errorDescription)
        {
            Error = error;
            ErrorDescription = errorDescription;
        }

        public string Error { get; set; }

        public string ErrorDescription { get; set; }
    }
}