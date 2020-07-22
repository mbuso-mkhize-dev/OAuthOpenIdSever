using Microsoft.AspNetCore.Identity;
using OAuthOpenIdServer.EntityFramework.Core.Interfaces;
using System;

namespace OAuthOpenIdServer.EntityFramework.Core.Models
{
    public class User : IdentityUser, ITimestamp
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsSystemUser { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public string ProfileImageUrlOriginal { get; set; }

        public string ProfileImageUrlLarge { get; set; }

        public string ProfileImageUrlMedium { get; set; }

        public string ProfileImageUrlSmall { get; set; }
    }
}