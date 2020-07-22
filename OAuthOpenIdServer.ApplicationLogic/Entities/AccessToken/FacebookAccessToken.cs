namespace OAuthOpenIdServer.ApplicationLogic.Entities.AccessToken
{
    public class FacebookAccessToken
    {
        public long ExpiresIn { get; set; }

        public string TokenType { get; set; }

        public string AccessToken { get; set; }
    }
}