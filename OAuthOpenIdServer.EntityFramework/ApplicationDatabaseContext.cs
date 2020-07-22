using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OAuthOpenIdServer.EntityFramework.Core.Interfaces;
using OAuthOpenIdServer.EntityFramework.Core.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OAuthOpenIdServer.EntityFramework
{
    public class ApplicationDatabaseContext : IdentityDbContext<User>
    {
        public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.UseOpenIddict();

            base.OnModelCreating(builder);
        }

        #region Overrides - Save Changes

        public override int SaveChanges()
        {
            SetTimestamps();

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetTimestamps();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            SetTimestamps();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SetTimestamps();

            return base.SaveChangesAsync(cancellationToken);
        }

        #endregion Overrides - Save Changes

        #region Private Methods

        private void SetTimestamps()
        {
            var changedEntries = ChangeTracker.Entries().Where(ce => ce.State == EntityState.Added ||
                                                                    ce.State == EntityState.Modified);

            foreach (var changedEntry in changedEntries)
            {
                var changeEntryType = changedEntry.Entity.GetType();

                var isTimestamp = changeEntryType.GetInterfaces().Any(i => i == typeof(ITimestamp));

                if (!isTimestamp) continue;

                PropertyInfo changeEntryPropertyInfo;

                if (changedEntry.State == EntityState.Added)
                {
                    changeEntryPropertyInfo = changeEntryType.GetProperty(nameof(ITimestamp.CreatedAt));
                    changeEntryPropertyInfo?.SetValue(changedEntry.Entity, DateTimeOffset.UtcNow, null);
                }

                changeEntryPropertyInfo = changeEntryType.GetProperty(nameof(ITimestamp.UpdatedAt));
                changeEntryPropertyInfo?.SetValue(changedEntry.Entity, DateTimeOffset.UtcNow, null);
            }
        }

        #endregion Private Methods
    }
}