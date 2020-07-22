namespace OAuthOpenIdServer.ApplicationLogic.Entities.ApplicationSettings
{
    public class EmailSettingsEntity
    {
        public SendGridSettingsEntity SendGrid { get; set; }

        public EmailAccountEntity AccountConfirmation { get; set; }

        public EmailAccountEntity AccountRecovery { get; set; }

        public FeedbackEntity SystemFeedback { get; set; }

        public FeedbackEntity PublicFeedback { get; set; }

        public class EmailAccountEntity
        {
            public string From { get; set; }

            public string FromName { get; set; }

            public string Subject { get; set; }
        }

        public class FeedbackEntity
        {
            public string To { get; set; }

            public string Subject { get; set; }
        }

        public class SendGridSettingsEntity
        {
            public string ApiKey { get; set; }
        }
    }
}