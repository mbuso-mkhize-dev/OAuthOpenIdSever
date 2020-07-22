using OAuthOpenIdServer.ApplicationLogic.Entities.AccessToken;
using OAuthOpenIdServer.EntityFramework.Core.Models;

namespace OAuthOpenIdServer.ApplicationLogic.Entities.Users
{
    public class FacebookUserEntity
    {
        public User IdentityUser { get; set; }

        public FacebookAccessToken FacebookAccessToken { get; set; }

        public FacebookUser FacebookUser { get; set; }
    }
}