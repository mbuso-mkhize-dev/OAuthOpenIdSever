using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OAuthOpenIdServer.ApplicationLogic.Boundaries.ApplicationLogic.Services;
using OAuthOpenIdServer.ApplicationLogic.Boundaries.EntityFramework.Repositories;
using OAuthOpenIdServer.ApplicationLogic.Entities.AccessToken;
using OAuthOpenIdServer.ApplicationLogic.Entities.ApplicationSettings;
using OAuthOpenIdServer.ApplicationLogic.Entities.Users;
using OAuthOpenIdServer.ApplicationLogic.Entities.Users.Mappers;
using OAuthOpenIdServer.ApplicationLogic.Exceptions;
using OAuthOpenIdServer.EntityFramework.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace OAuthOpenIdServer.ApplicationLogic.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IBaseRepository<User> _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly IOptions<ExternalAuthSettingsEntity> _externalAuthSettings;

        public UserService(
            UserManager<User> userManager,
            IBaseRepository<User> userRepository,
            SignInManager<User> signInManager,
            IOptions<ExternalAuthSettingsEntity> externalAuthSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _externalAuthSettings = externalAuthSettings;
        }

        public async Task CheckPulseAsync()
        {
            var userCount = await _userRepository.CountAsync();
        }

        public async Task<UserEntity> CreateUserAsync(UserEntity publicUserEntity, string baseCallbackUrl)
        {
            if (string.IsNullOrWhiteSpace(publicUserEntity?.Email))
                throw new ApplicationInvalidDataException("Email cannot be empty.");

            if (string.IsNullOrWhiteSpace(publicUserEntity?.Password))
                throw new ApplicationInvalidDataException("Password cannot be empty.");

            var user = UserEntityMapper.MapToPublicUserModel(publicUserEntity);

            var existingUser = await _userManager.FindByEmailAsync(publicUserEntity.Email);

            if (existingUser != null)
            {
                throw new ApplicationDuplicateDataException("Email already registered.");
            }

            existingUser = await _userManager.FindByNameAsync(publicUserEntity.Username);

            if (existingUser != null)
            {
                throw new ApplicationDuplicateDataException("Username already registered.");
            }

            var identityResult = await _userManager.CreateAsync(user, publicUserEntity.Password);

            if (!identityResult.Succeeded)
            {
                var error = identityResult.Errors.FirstOrDefault();

                throw new ApplicationInvalidDataException(error.Description);
            }
            else
            {
                //var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //var callbackUrl = BuildCallbackUrlWithParameters(baseCallbackUrl, user.Id, confirmToken);

                //await _emailSender.SendAccountConfirmationEmailAsync(callbackUrl, user.Email);

                return UserEntityMapper.MapToEntity(user);
            }
        }

        public async Task<UserEntity> GetUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ApplicationInvalidDataException("Id cannot be empty.");

            var user = await _userRepository.FindAsync(userId);

            if (user == null)
                throw new ApplicationObjectNotFoundException("User does not exist.");

            return UserEntityMapper.MapToEntity(user);
        }

        public async Task<UserEntity> UpdateUserAsync(string userId, string username, string mobileNumber, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ApplicationInvalidDataException("Id cannot be empty.");

            if (string.IsNullOrWhiteSpace(username))
                throw new ApplicationInvalidDataException("Username cannot be empty.");

            var user = await _userRepository.FindAsync(userId);

            if (user == null)
                throw new ApplicationObjectNotFoundException("User does not exist.");

            if (!user.UserName.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _userManager.FindByNameAsync(username);

                if (existingUser != null)
                {
                    throw new ApplicationDuplicateDataException("Username already registered.");
                }

                user.UserName = username;
            }

            user.LastName = lastName;
            user.FirstName = firstName;
            user.PhoneNumber = mobileNumber;
            user.PhoneNumberConfirmed = false;

            await _userManager.UpdateNormalizedUserNameAsync(user);

            await _userRepository.SaveAsync();

            return UserEntityMapper.MapToEntity(user);
        }

        public async Task<bool> DoesPublicAccountExistAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ApplicationInvalidDataException("Email cannot be empty.");

            var user = await _userManager.FindByEmailAsync(email);

            return user != null && !user.IsSystemUser;
        }

        public async Task<string> SendPublicAccountRecoveryEmailAsync(string email, string baseCallbackUrl)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ApplicationInvalidDataException("Email cannot be empty.");

            var user = await _userManager.FindByEmailAsync(email);

            if (user != null && !user.IsSystemUser)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = BuildCallbackUrlWithParameters(baseCallbackUrl, user.Id, resetToken);

                //await _emailSender.SendAccountRecoveryEmailAsync(callbackUrl, email);

                return callbackUrl;
            }
            else
            {
                throw new ApplicationObjectNotFoundException("User does not exist.");
            }
        }

        public async Task<string> ResendPublicAccountVerificationEmailAsync(string userId, string baseCallbackUrl)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ApplicationInvalidDataException("Id cannot be empty.");

            var user = await _userRepository.FindAsync(userId);

            if (user == null)
                throw new ApplicationObjectNotFoundException("User does not exist.");

            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = BuildCallbackUrlWithParameters(baseCallbackUrl, user.Id, confirmToken);

            //await _emailSender.SendAccountConfirmationEmailAsync(callbackUrl, user.Email);

            return callbackUrl;
        }

        public async Task<IdentityResult> ResetPublicAccountPasswordAsync(string userId, string code, string password)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ApplicationInvalidDataException("Id cannot be empty.");

            if (string.IsNullOrWhiteSpace(code))
                throw new ApplicationInvalidDataException("Reset code cannot be empty.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationInvalidDataException("Password cannot be empty.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            if (user.IsSystemUser)
                return null;

            return await _userManager.ResetPasswordAsync(user, code, password);
        }

        public async Task<IdentityResult> ChangePublicAccountPasswordAsync(string userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ApplicationInvalidDataException("Id cannot be empty.");

            if (string.IsNullOrWhiteSpace(currentPassword))
                throw new ApplicationInvalidDataException("Current password cannot be empty.");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ApplicationInvalidDataException("New password cannot be empty.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new ApplicationObjectNotFoundException("User does not exist.");

            if (user.IsSystemUser)
                throw new ApplicationObjectNotFoundException("User does not exist.");

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return changePasswordResult;
        }

        public async Task<IdentityResult> ChangeSystemAccountPasswordAsync(string userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ApplicationInvalidDataException("Id cannot be empty.");

            if (string.IsNullOrWhiteSpace(currentPassword))
                throw new ApplicationInvalidDataException("Current password cannot be empty.");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ApplicationInvalidDataException("New password cannot be empty.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new ApplicationObjectNotFoundException("User does not exist.");

            if (!user.IsSystemUser)
                throw new ApplicationObjectNotFoundException("User does not exist.");

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return changePasswordResult;
        }

        private string BuildCallbackUrlWithParameters(string baseCallbackUrl, string userId, string code)
        {
            var uriBuilder = new UriBuilder(baseCallbackUrl);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["userId"] = userId;
            query["code"] = code;

            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }

        public async Task<IdentityResult> ConfirmPublicAccountAsync(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ApplicationInvalidDataException("Id cannot be empty.");

            if (string.IsNullOrWhiteSpace(code))
                throw new ApplicationInvalidDataException("Confirmation code cannot be empty.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            if (user.IsSystemUser)
                return null;

            return await _userManager.ConfirmEmailAsync(user, code);
        }

        public async Task<FacebookUserEntity> RegisterFacebookUserAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ApplicationInvalidDataException("Access Token cannot be empty.");

            //var facebookAccessToken = await _facebookOAuthEndpoint.ExchangeTokenAsync(
            //    _externalAuthSettings.Value.Facebook.AppId,
            //    _externalAuthSettings.Value.Facebook.AppSecret,
            //    accessToken);

            //if (facebookAccessToken == null)
            //    throw new ApplicationUnauthorizedException("Invalid facebook access token.");

            //var facebookUser = await _facebookMeEndpoint.GetFacebookUserAsync(facebookAccessToken.AccessToken);

            //if (string.IsNullOrWhiteSpace(facebookUser?.Id))
            //    throw new ApplicationUnauthorizedException("Invalid facebook access token.");

            //var facebookProfileImageUrl = await _facebookPictureEndpoint.GetFacebookProfileImageUrlAsync(facebookUser.Id);

            //var userLoginInfo = new UserLoginInfo("Facebook", facebookUser.Id, "Facebook");

            //var externalLoginResult = await _signInManager.ExternalLoginSignInAsync("Facebook", facebookUser.Id, isPersistent: false);

            var userLoginInfo = new UserLoginInfo("Facebook", string.Empty, "Facebook");

            var externalLoginResult = await _signInManager.ExternalLoginSignInAsync("Facebook", string.Empty, isPersistent: false);

            User user = null;

            if (!externalLoginResult.Succeeded)
            {
                if (externalLoginResult.IsLockedOut)
                    throw new ApplicationUnauthorizedException("Account has been suspended.");

                if (externalLoginResult.IsNotAllowed)
                    throw new ApplicationUnauthorizedException("Login not allowed.");

                if (externalLoginResult.RequiresTwoFactor)
                    throw new ApplicationUnauthorizedException("Two factor not supported.");

                //user = new User
                //{
                //    Email = facebookUser.Email,
                //    Active = true,
                //    IsSystemUser = false,
                //    UserName = facebookUser.Name.Replace(" ", ""),
                //    FirstName = facebookUser.FirstName,
                //    LastName = facebookUser.LastName
                //};

                //if (!string.IsNullOrWhiteSpace(facebookProfileImageUrl))
                //    user.ProfileImageUrlOriginal = facebookProfileImageUrl;

                var userResult = await _userManager.CreateAsync(user);

                var loginResult = await _userManager.AddLoginAsync(user, userLoginInfo);
            }
            else
            {
                //user = await _userManager.FindByLoginAsync("Facebook", facebookUser.Id);

                //if (!string.IsNullOrWhiteSpace(facebookProfileImageUrl))
                //{
                //    user.ProfileImageUrlOriginal = facebookProfileImageUrl;

                //    await _userManager.UpdateAsync(user);
                //}
            }

            return new FacebookUserEntity
            {
                IdentityUser = user,
                //FacebookUser = facebookUser,
                //FacebookAccessToken = facebookAccessToken
            };
        }

        public async Task UpdateFacebookAuthenticationTokensAsync(
            ClaimsPrincipal principal,
            FacebookAccessToken facebookAccessToken,
            string providerKey)
        {
            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ApplicationInvalidDataException("Provider Key cannot be empty.");

            //var authenticationTokens = new List<AuthenticationToken>()
            //{
            //    new AuthenticationToken { Name = "access_token", Value = facebookAccessToken.AccessToken },
            //    new AuthenticationToken { Name = "token_type", Value = facebookAccessToken.TokenType },
            //};

            //if (facebookAccessToken.ExpiresIn > 0)
            //{
            //    authenticationTokens.Add(new AuthenticationToken
            //    {
            //        Name = "expires_at",
            //        Value = DateTimeOffset.UtcNow.AddSeconds(facebookAccessToken.ExpiresIn).ToString("o")
            //    });
            //}

            //var externalLoginInfo = new ExternalLoginInfo(principal, "Facebook", providerKey, "Facebook")
            //{
            //    AuthenticationTokens = authenticationTokens
            //};

            //var updateExternalTokensResult = await _signInManager.UpdateExternalAuthenticationTokensAsync(externalLoginInfo);

            //if (!updateExternalTokensResult.Succeeded)
            //{
            //    throw new ApplicationUnauthorizedException(updateExternalTokensResult.Errors.FirstOrDefault()?.Description);
            //}
        }

        public IEnumerable<UserEntity> SearchPublicUsers(string query, int page, int limit)
        {
            var users = _userRepository.Where(
                user => !user.IsSystemUser &&
                (string.IsNullOrWhiteSpace(query) ||
                (!string.IsNullOrWhiteSpace(user.Email) && user.Email.ToLower().Contains(query.ToLower())) ||
                (!string.IsNullOrWhiteSpace(user.UserName) && user.UserName.ToLower().Contains(query.ToLower())) ||
                (!string.IsNullOrWhiteSpace(user.LastName) && user.LastName.ToLower().Contains(query.ToLower())) ||
                (!string.IsNullOrWhiteSpace(user.FirstName) && user.FirstName.ToLower().Contains(query.ToLower()))))
                .OrderByDescending(u => u.CreatedAt)
                .Skip(page * limit)
                .Take(limit)
                .ToList();

            return UserEntityMapper.MapToEntities(users);
        }

        public async Task<UserEntity> SuspendPublicUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ApplicationInvalidDataException("User id cannot be empty.");

            var user = await _userRepository.FindAsync(id);

            if (user == null)
                throw new ApplicationObjectNotFoundException("User does not exist.");

            user.Active = false;

            await _userRepository.SaveAsync();

            // Update security stamp in order to invalidate all tokens
            await _userManager.UpdateSecurityStampAsync(user);

            return UserEntityMapper.MapToEntity(user);
        }

        public async Task<UserEntity> UnsuspendPublicUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ApplicationInvalidDataException("User id cannot be empty.");

            var user = await _userRepository.FindAsync(id);

            if (user == null)
                throw new ApplicationObjectNotFoundException("User does not exist.");

            user.Active = true;

            await _userRepository.SaveAsync();

            // Update security stamp in order to invalidate all tokens
            await _userManager.UpdateSecurityStampAsync(user);

            return UserEntityMapper.MapToEntity(user);
        }
    }
}