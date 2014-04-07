
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.AspNet.Identity.Entity
{
    public class EntityUser : EntityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public EntityUser()
        {
            // TODO: Remove/move Id to Create?
            Id = Guid.NewGuid().ToString();
        }

        public EntityUser(string userName) : this()
        {
            UserName = userName;
        }
    }

    public class EntityUser<TKey, TLogin, TRole, TClaim> : IdentityUser<TKey>
        where TLogin : IdentityUserLogin<TKey>
        where TRole : IdentityUserRole<TKey>
        where TClaim : IdentityUserClaim<TKey>
        where TKey : IEquatable<TKey>
    {
        public EntityUser()
        {
            Claims = new List<TClaim>();
            Roles = new List<TRole>();
            Logins = new List<TLogin>();
        }

        /// <summary>
        ///     DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public new virtual DateTime? LockoutEnd { get; set; }

        /// <summary>
        ///     Navigation property for user roles
        /// </summary>
        public new virtual ICollection<TRole> Roles { get; private set; }

        /// <summary>
        ///     Navigation property for user claims
        /// </summary>
        public new virtual ICollection<TClaim> Claims { get; private set; }

        /// <summary>
        ///     Navigation property for user logins
        /// </summary>
        public new virtual ICollection<TLogin> Logins { get; private set; }
    }
}
