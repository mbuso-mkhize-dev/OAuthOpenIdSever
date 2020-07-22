

using OAuthOpenIdServer.EntityFramework.Core.Models;

namespace AuthServer.Models
{
    public class RegisterResponseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public RegisterResponseViewModel(User user)
        {
            Id = user.Id;
            Name = user.FirstName;
            Email = user.Email;
        }
    }
}
