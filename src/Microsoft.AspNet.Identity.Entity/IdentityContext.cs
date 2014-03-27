using System;
using Microsoft.Data.Entity;
using Microsoft.Data.InMemory;
using Microsoft.Data.SqlServer;
using Microsoft.Data.Entity.Metadata;

namespace Microsoft.AspNet.Identity.Entity
{
    public class IdentityContext : IdentityContext<string> { }

    public class IdentityContext<TKey> : EntityContext where TKey : IEquatable<TKey>
    {

        public EntitySet<IdentityUser<TKey>> Users { get; set; }
        public EntitySet<IdentityRole<TKey>> Roles { get; set; }

        protected override void OnConfiguring(EntityConfigurationBuilder builder)
        {
#if NET45
            builder.UseSqlServer(@"Server=(localdb)\v11.0;Database=IdentityDb;Trusted_Connection=True;");
#else
            builder.UseDataStore(new InMemoryDataStore());
#endif
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityUser<TKey>>().Key(u => u.Id);
            builder.Entity<IdentityRole<TKey>>().Key(r => r.Id);
        }

    }
}