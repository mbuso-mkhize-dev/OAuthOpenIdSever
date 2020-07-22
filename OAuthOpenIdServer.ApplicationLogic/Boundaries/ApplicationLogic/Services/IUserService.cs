using Microsoft.AspNetCore.Identity;
using OAuthOpenIdServer.ApplicationLogic.Entities.AccessToken;
using OAuthOpenIdServer.ApplicationLogic.Entities.Users;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuthOpenIdServer.ApplicationLogic.Boundaries.ApplicationLogic.Services
{
    public interface IUserService
    {
        Task CheckPulseAsync();

        Task<UserEntity> CreateUserAsync(UserEntity publicUserEntity, string baseCallbackUrl);

        Task<UserEntity> GetUserAsync(string userId);

        Task<UserEntity> UpdateUserAsync(string userId, string username, string mobileNumber, string firstName, string lastName);

        Task<bool> DoesPublicAccountExistAsync(string email);

        Task<string> SendPublicAccountRecoveryEmailAsync(string email, string baseCallbackUrl);

        Task<string> ResendPublicAccountVerificationEmailAsync(string userId, string baseCallbackUrl);

        Task<IdentityResult> ResetPublicAccountPasswordAsync(string userId, string code, string password);

        Task<IdentityResult> ChangePublicAccountPasswordAsync(string userId, string currentPassword, string newPassword);

        Task<IdentityResult> ChangeSystemAccountPasswordAsync(string userId, string currentPassword, string newPassword);

        IEnumerable<UserEntity> SearchPublicUsers(string query, int page, int limit);

        Task<UserEntity> SuspendPublicUserAsync(string id);

        Task<UserEntity> UnsuspendPublicUserAsync(string id);

        Task<IdentityResult> ConfirmPublicAccountAsync(string userId, string code);

        Task<FacebookUserEntity> RegisterFacebookUserAsync(string accessToken);

        Task UpdateFacebookAuthenticationTokensAsync(ClaimsPrincipal principal, FacebookAccessToken facebookAccessToken, string providerKey);
    }
}