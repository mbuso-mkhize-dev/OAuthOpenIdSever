namespace OAuthOpenIdServer.ApplicationLogic.Entities.ApplicationSettings
{
    public class AuthSettingsEntity
    {
        public string Host { get; set; }

        public string Issuer { get; set; }

        public AudienceEntity Audience { get; set; }

        public AccessTokenEntity AccessToken { get; set; }

        public RefreshTokenEntity RefreshToken { get; set; }

        public class AudienceEntity
        {
            public string Id { get; set; }

            public string Secret { get; set; }
        }

        public class AccessTokenEntity
        {
            public int LifetimeInSeconds { get; set; }
        }

        public class RefreshTokenEntity
        {
            public int LifetimeInSeconds { get; set; }
        }
    }
}