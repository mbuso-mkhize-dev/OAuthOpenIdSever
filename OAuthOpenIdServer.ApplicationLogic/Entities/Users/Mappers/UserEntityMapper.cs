using OAuthOpenIdServer.EntityFramework.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace OAuthOpenIdServer.ApplicationLogic.Entities.Users.Mappers
{
    /// <summary>
    /// User model mapper
    /// </summary>
    public static class UserEntityMapper
    {
        /// <summary>
        /// Map to user model
        /// </summary>
        /// <param name="userEntity">The source entity</param>
        /// <returns></returns>
        public static User MapToPublicUserModel(UserEntity userEntity)
        {
            return new User
            {
                Active = true,
                IsSystemUser = false,
                Email = userEntity.Email,
                UserName = userEntity.Username,
                LastName = userEntity.LastName,
                FirstName = userEntity.FirstName,
                PhoneNumber = userEntity.MobileNumber
            };
        }

        /// <summary>
        /// Map to user entity
        /// </summary>
        /// <param name="sourceModel">The source database model</param>
        /// <returns></returns>
        public static UserEntity MapToEntity(User sourceModel)
        {
            return new UserEntity
            {
                Id = sourceModel.Id,
                Email = sourceModel.Email,
                Active = sourceModel.Active,
                LastName = sourceModel.LastName,
                Username = sourceModel.UserName,
                FirstName = sourceModel.FirstName,
                MobileNumber = sourceModel.PhoneNumber,
                EmailConfirmed = sourceModel.EmailConfirmed
            };
        }

        /// <summary>
        /// Map to collection of user entities
        /// </summary>
        /// <param name="sourceModels">The collection of source database models</param>
        /// <returns></returns>
        public static IEnumerable<UserEntity> MapToEntities(IEnumerable<User> sourceModels = null)
        {
            return sourceModels?.Select(MapToEntity) ?? Enumerable.Empty<UserEntity>();
        }
    }
}