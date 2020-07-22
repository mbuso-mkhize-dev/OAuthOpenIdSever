using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;

namespace OAuthOpenIdServer.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public bool RememberLogin { get; set; }
        public string ExternalLoginScheme { get; set; }
        public bool IsExternalLoginOnly { get; set; }
        public bool EnableLocalLogin { get; set; }

        public bool AllowRememberLogin { get; set; }

        public List<ExternalProvider> VisibleExternalProviders { get; set; } = new List<ExternalProvider>();
    }
}