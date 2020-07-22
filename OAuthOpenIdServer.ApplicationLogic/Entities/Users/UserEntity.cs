namespace OAuthOpenIdServer.ApplicationLogic.Entities.Users
{
    public abstract class BaseUserEntity
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string MobileNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class UserEntity : BaseUserEntity
    {
        public string Id { get; set; }

        public bool Active { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool HasSubscriptions { get; set; }
        public string Password { get; set; }
    }

    public class CreatePublicUserEntity : BaseUserEntity
    {
        public string Password { get; set; }
    }
}