using System;

namespace OAuthOpenIdServer.EntityFramework.Core.Interfaces
{
    public interface ITimestamp
    {
        DateTimeOffset CreatedAt { get; set; }

        DateTimeOffset UpdatedAt { get; set; }
    }
}