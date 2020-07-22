using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthOpenIdServer.Extensions
{
    public static class ConfigurationHelper
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
              new List<IdentityResource>
              {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "rc.scope",
                    UserClaims =
                    {
                        "rc.garndma",
                        "api.read"
                    }
                }
              };

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> {
                new ApiResource("apiopen"),
                new ApiResource("api.read"),
                // new ApiResource("resourceapi", "Resource API")
                //{
                //    Scopes = new[] { "api.read"}
                //}
            };

        public static IEnumerable<ApiScope> ApiScopes =>
           new List<ApiScope>
           {
                new ApiScope("api1", "My API")
           };

        public static IEnumerable<Client> GetClients(string clientId, string clientSecret) =>
            new List<Client> {

                new Client {
                    ClientId = clientId,
                    ClientSecrets = { new Secret(clientSecret.ToSha256()) },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    //RequirePkce = true,

                    RedirectUris = { "https://localhost:44322/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44322/Home/Index" },

                    AllowedScopes = {
                        "apiopen"
                    },

                    // puts all the claims in the id token
                    //AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                },
                new Client {
                    RequireConsent = false,
                    ClientId = "angular_spa",
                    ClientName = "Angular SPA",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes = { "openid", "profile", "email",  "api1"},
                    RedirectUris = {"http://localhost:4200/auth-callback"},
                    PostLogoutRedirectUris = {"http://localhost:4200/"},
                    AllowedCorsOrigins = {"http://localhost:4200"},
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 3600,
                    RequireClientSecret = false
                }
            };
       
    }
}
