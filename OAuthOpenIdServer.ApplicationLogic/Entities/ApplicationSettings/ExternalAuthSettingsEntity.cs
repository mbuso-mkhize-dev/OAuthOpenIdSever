namespace OAuthOpenIdServer.ApplicationLogic.Entities.ApplicationSettings
{
    public class ExternalAuthSettingsEntity
    {
        public FacebookSettingsEntity Facebook { get; set; }

        public class FacebookSettingsEntity
        {
            public string AppId { get; set; }

            public string AppSecret { get; set; }
        }
    }
}