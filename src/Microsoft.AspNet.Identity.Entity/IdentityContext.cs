using System;
using Microsoft.Data.Entity;
using Microsoft.Data.InMemory;
using Microsoft.Data.SqlServer;
using Microsoft.Data.Entity.Metadata;

namespace Microsoft.AspNet.Identity.Entity
{
    public class IdentityContext : IdentityContext<IdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim> { }

    public class IdentityContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : EntityContext
        where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRole<TKey, TUserRole> /*, TUserRole*/
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TKey : IEquatable<TKey>
    {

        public EntitySet<TUser> Users { get; set; }
        public EntitySet<TRole> Roles { get; set; }

        protected override void OnConfiguring(EntityConfigurationBuilder builder)
        {
//#if NET45
//            builder.UseSqlServer(@"Server=(localdb)\v11.0;Database=IdentityDb;Trusted_Connection=True;");
//#else
            builder.UseDataStore(new InMemoryDataStore());
//#endif
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // TODO: Would be nice if we could just do Entity<IUser<TKey>> instead of forcing the concrete type
            builder.Entity<TUser>()
                .Key(u => u.Id)
                .Properties(ps => ps.Property(u => u.UserName));
            //builder.Entity<TRole>()
            //    .Key(r => r.Id);
 
            builder.Entity<TUserRole>()
                .Key(r => new {r.UserId, r.RoleId})
                .ForeignKeys(fk => fk.ForeignKey<TUser>(f => f.UserId))
                .ForeignKeys(fk => fk.ForeignKey<TRole>(f => f.RoleId));
                //.ToTable("AspNetUserRoles");

            builder.Entity<TUserLogin>()
                .Key(l => new {l.LoginProvider, l.ProviderKey, l.UserId});
            //.ToTable("AspNetUserLogins");
        }

    }
}