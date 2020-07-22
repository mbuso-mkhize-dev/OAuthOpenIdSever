//using AspNet.Security.OpenIdConnect.Extensions;
//using AspNet.Security.OpenIdConnect.Primitives;
//using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAuthOpenIdServer.ApplicationLogic.Boundaries.ApplicationLogic.Services;
using OAuthOpenIdServer.ApplicationLogic.Boundaries.EntityFramework.Repositories;
using OAuthOpenIdServer.ApplicationLogic.Entities.ApplicationSettings;
using OAuthOpenIdServer.ApplicationLogic.Exceptions;
using OAuthOpenIdServer.EntityFramework.Core.Models;
using OAuthOpenIdServer.Filters;
//using OpenIddict.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthOpenIdServer.Controllers
{
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiExceptionFilter]
    public class OAuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IBaseRepository<User> _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly IOptions<AuthSettingsEntity> _authSettings;
        //private readonly OpenIddictApplicationManager<OpenIddictApplicationStoreResolver> _applicationManager;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="userRepository">User repository</param>
        /// <param name="userService">User service</param>
        /// <param name="signInManager">Identity sign in manager</param>
        /// <param name="identityOptions">Identity options</param>
        /// <param name="authSettings">Auth settings</param>
        /// <param name="applicationManager">Open Id Connect application manager</param>
        public OAuthController(
            IUserService userService,
            UserManager<User> userManager,
            IBaseRepository<User> userRepository,
            SignInManager<User> signInManager,
            IOptions<IdentityOptions> identityOptions,
            IOptions<AuthSettingsEntity> authSettings)
            //OpenIddictApplicationManager<OpenIddictApplicationStoreResolver> applicationManager)
        {
            _userService = userService;
            _userManager = userManager;
            _authSettings = authSettings;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _identityOptions = identityOptions;
            //_applicationManager = applicationManager;
        }

        ///// <summary>
        ///// Exchange credentials for token
        ///// </summary>
        ///// <param name="request">Open Id Connect request</param>
        ///// <returns></returns>
        //[Produces("application/json")]
        //[HttpPost("~/oauth/token")]
        //public async Task<IActionResult> ExchangeToken(OpenIdConnectRequest request)
        //{
        //    if (request.IsPasswordGrantType())
        //    {
        //        var userResult = await _userManager.FindByNameAsync(request.Username);

        //        if (userResult == null)
        //        {
        //            // If finding by name fails, let's try email address.
        //            userResult = await _userManager.FindByEmailAsync(request.Username);
        //        }

        //        // Couldn't find user by username or email address.
        //        if (userResult == null)
        //        {
        //            return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.InvalidGrant, "Incorrect username and password combination.");
        //        }

        //        //// System users should not be allowed (public api)
        //        //if (userResult.IsSystemUser)
        //        //{
        //        //    return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.InvalidGrant, "Incorrect username and password combination.");
        //        //}

        //        //if (!userResult.Active)
        //        //{
        //        //    return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.AccessDenied, "Account has been suspended.");
        //        //}

        //        // Validate the username/password parameters and ensure the account is not locked out.
        //        var signInResult = await _signInManager.CheckPasswordSignInAsync(userResult, request.Password, lockoutOnFailure: false);

        //        if (!signInResult.Succeeded)
        //        {
        //            return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.InvalidGrant, "Incorrect username and password combination.");
        //        }

        //        // Create a new authentication ticket.
        //        var ticket = await CreateTicketAsync(
        //            request: request,
        //            user: userResult);

        //        return SignIn(
        //            ticket.Principal,
        //            ticket.Properties,
        //            ticket.AuthenticationScheme);
        //    }
        //    else if (request.IsRefreshTokenGrantType())
        //    {
        //        // Retrieve the claims principal stored in the refresh token.
        //        var info = await HttpContext.AuthenticateAsync(OpenIdConnectServerDefaults.AuthenticationScheme);

        //        var userResult = await _userManager.GetUserAsync(info.Principal);

        //        if (userResult == null)
        //        {
        //            return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.InvalidGrant, "The refresh token is no longer valid.");
        //        }

        //        // System users should not be allowed (public api)
        //        //if (userResult.IsSystemUser)
        //        //{
        //        //    return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.InvalidGrant, "The refresh token is no longer valid.");
        //        //}

        //        //if (!userResult.Active)
        //        //{
        //        //    return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.AccessDenied, "Account has been suspended.");
        //        //}

        //        // Ensure the user is still allowed to sign in.
        //        if (!await _signInManager.CanSignInAsync(userResult))
        //        {
        //            return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.InvalidGrant, "The user is no longer allowed to sign in.");
        //        }

        //        // Create a new authentication ticket, but reuse the properties stored
        //        // in the refresh token, including the scopes originally granted.
        //        var ticket = await CreateTicketAsync(
        //            request: request,
        //            user: userResult,
        //            properties: info.Properties);

        //        return SignIn(
        //            ticket.Principal,
        //            ticket.Properties,
        //            ticket.AuthenticationScheme);
        //    }
        //    else if (request.GrantType.Equals("urn:ietf:params:oauth:grant-type:facebook_access_token") ||
        //        System.Net.WebUtility.UrlDecode(request.GrantType).Equals("urn:ietf:params:oauth:grant-type:facebook_access_token"))
        //    {
        //        if (string.IsNullOrWhiteSpace(request.Assertion))
        //        {
        //            return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.UnsupportedGrantType, "The mandatory 'assertion' parameter is missing.");
        //        }

        //        try
        //        {
        //            var externalUserResult = await _userService.RegisterFacebookUserAsync(request.Assertion);

        //            // Create a new authentication ticket.
        //            var ticket = await CreateTicketAsync(
        //                request: request,
        //                user: externalUserResult.IdentityUser);

        //            await _userService.UpdateFacebookAuthenticationTokensAsync(
        //                ticket.Principal,
        //                externalUserResult.FacebookAccessToken,
        //                externalUserResult.FacebookUser.Id);

        //            return SignIn(
        //                ticket.Principal,
        //                ticket.Properties,
        //                ticket.AuthenticationScheme);
        //        }
        //        catch (ApplicationUnauthorizedException e)
        //        {
        //            var error = e.Message ?? "Login not allowed.";

        //            return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.AccessDenied, "Login not allowed.");
        //        }
        //    }
        //    return GetBadRequestOpenIdConnectResponse(OpenIdConnectConstants.Errors.UnsupportedGrantType, "The specified 'grant_type' is not supported.");
        //}

        //private IActionResult GetBadRequestOpenIdConnectResponse(string error, string errorDescription)
        //{
        //    return BadRequest(new OpenIdConnectResponse
        //    {
        //        Error = error,
        //        ErrorDescription = errorDescription
        //    });
        //}

        //private async Task<AuthenticationTicket> CreateTicketAsync(
        //    OpenIdConnectRequest request,
        //    User user,
        //    AuthenticationProperties properties = null)
        //{
        //    // Create a new ClaimsPrincipal containing the claims that
        //    // will be used to create an id_token, a token or a code.
        //    var principal = await _signInManager.CreateUserPrincipalAsync(user);

        //    // Create a new authentication ticket holding the user identity.
        //    var authenticationProperties = properties ?? new AuthenticationProperties();
        //    var ticket = new AuthenticationTicket(principal,
        //        authenticationProperties,
        //        OpenIdConnectServerDefaults.AuthenticationScheme);

        //    // Set the list of scopes granted to the client application.
        //    if (!request.IsRefreshTokenGrantType())
        //    {
        //        ticket.SetScopes(new[]
        //        {
        //            OpenIdConnectConstants.Scopes.OfflineAccess
        //        }.Intersect(request.GetScopes()));
        //    }

        //    ticket.SetAudiences(request.ClientId);
        //    ticket.SetResources(_authSettings.Value.Audience.Id);

        //    // Note: by default, claims are NOT automatically included in the access and identity tokens.
        //    // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        //    // whether they should be included in access tokens, in identity tokens or in both.

        //    foreach (var claim in ticket.Principal.Claims)
        //    {
        //        // Never include the security stamp in the access and identity tokens, as it's a secret value.
        //        if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType)
        //        {
        //            continue;
        //        }

        //        var destinations = new List<string>
        //        {
        //            OpenIdConnectConstants.Destinations.AccessToken
        //        };

        //        //// Only add the iterated claim to the id_token if the corresponding scope was granted to the client application.
        //        //// The other claims will only be added to the access_token, which is encrypted when using the default format.
        //        //if ((claim.Type == OpenIdConnectConstants.Claims.Name && ticket.HasScope(OpenIdConnectConstants.Scopes.Profile)) ||
        //        //    (claim.Type == OpenIdConnectConstants.Claims.Email && ticket.HasScope(OpenIdConnectConstants.Scopes.Email)) ||
        //        //    (claim.Type == OpenIdConnectConstants.Claims.Role && ticket.HasScope(OpenIddictConstants.Claims.Roles)))
        //        //{
        //        //    destinations.Add(OpenIdConnectConstants.Destinations.IdentityToken);
        //        //}

        //        claim.SetDestinations(destinations);
        //    }

        //    return ticket;
        //}
    }
}