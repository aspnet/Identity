using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace Microsoft.AspNet.Identity.Entity
{
    public class UserStore :
        UserStore<IdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public UserStore(EntityContext context) : base(context) { }
    }

    public class UserStore<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> :
        IUserLoginStore<TUser, TKey>,
        IUserClaimStore<TUser, TKey>,
        IUserRoleStore<TUser, TKey>,
        IUserPasswordStore<TUser, TKey>,
        IUserSecurityStampStore<TUser, TKey>,
        IQueryableUserStore<TUser, TKey>,
        IUserEmailStore<TUser, TKey>,
        IUserPhoneNumberStore<TUser, TKey>,
        IUserTwoFactorStore<TUser, TKey>,
        IUserLockoutStore<TUser, TKey>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRole<TKey/*, TUserRole*/>
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserClaim : IdentityUserClaim<TKey>, new()
    {
        private bool _disposed;

        public UserStore(EntityContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            AutoSaveChanges = true;
        }

        public EntityContext Context { get; private set; }

        /// <summary>
        ///     If true will call SaveChanges after Create/Update/Delete
        /// </summary>
        public bool AutoSaveChanges { get; set; }

        private async Task SaveChanges()
        {
            if (AutoSaveChanges)
            {
                await Context.SaveChangesAsync();
            }
        }

        public virtual Task<TUser> GetUserAggregate(Expression<Func<TUser, bool>> filter)
        {
            return Users.SingleOrDefaultAsync(filter);
                //Include(u => u.Roles)
                //.Include(u => u.Claims)
                //.Include(u => u.Logins)
        }

        public async virtual Task Create(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            await Context.AddAsync(user);
            await SaveChanges();
        }

        public async virtual Task Update(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            await Context.UpdateAsync(user);
            await SaveChanges();
        }

        public async virtual Task Delete(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            // TODO: why is there no DeleteAsync?
            Context.Delete(user);
            await SaveChanges();
        }

        /// <summary>
        ///     Find a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindById(TKey userId)
        {
            ThrowIfDisposed();
            // TODO: FirstOrDeafult with query throws an exception with Single
            return Task.FromResult(Users.Where(u => u.Id.Equals(userId)).FirstOrDefault());
            //return GetUserAggregate(u => u.Id.Equals(userId));
        }

        /// <summary>
        ///     Find a user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByName(string userName)
        {
            ThrowIfDisposed();
            // TODO: FirstOrDeafult with query throws an exception with Single
            return Task.FromResult(Users.Where(u => u.UserName.ToUpper() == userName.ToUpper()).SingleOrDefault());
            //return GetUserAggregate(u => u.UserName.ToUpper() == userName.ToUpper());
        }

        public IQueryable<TUser> Users
        {
            get { return Context.Set<TUser>(); }
        }

        public Task AddLogin(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            user.Logins.Add(new TUserLogin
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider
            });
            return Task.FromResult(0);
        }

        public Task RemoveLogin(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLogins(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            IList<UserLoginInfo> result =
                user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList();
            return Task.FromResult(result);
        }

        public async virtual Task<TUser> Find(UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            var provider = login.LoginProvider;
            var key = login.ProviderKey;
            var userLogin =
                Context.Set<TUserLogin>().FirstOrDefault(l => l.LoginProvider == provider && l.ProviderKey == key);
            if (userLogin != null)
            {
                return await GetUserAggregate(u => u.Id.Equals(userLogin.UserId));
            }
            return null;
        }

        /// <summary>
        ///     Set the password hash for a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task SetPasswordHash(TUser user, string passwordHash)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get the password hash for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHash(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        ///     Returns true if the user has a password set
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> HasPassword(TUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        /// <summary>
        ///     Return the claims for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<IList<Claim>> GetClaims(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            IList<Claim> result = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        /// <summary>
        ///     Add a claim to a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual Task AddClaim(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            user.Claims.Add(new TUserClaim { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Remove a claim from a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public virtual Task RemoveClaim(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns whether the user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmed(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        ///     Set IsConfirmed on the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetEmailConfirmed(TUser user, bool confirmed)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Set the user email
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task SetEmail(TUser user, string email)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Email = email;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get the user's email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetEmail(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Email);
        }

        /// <summary>
        ///     Find a user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<TUser> FindByEmail(string email)
        {
            ThrowIfDisposed();
            return Task.FromResult(Users.Where(u => u.Email.ToUpper() == email.ToUpper()).SingleOrDefault());
            //return GetUserAggregate(u => u.Email.ToUpper() == email.ToUpper());
        }

        /// <summary>
        ///     Returns the DateTimeOffset that represents the end of a user's lockout, any time in the past should be considered
        ///     not locked out.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<DateTimeOffset> GetLockoutEndDate(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }

        /// <summary>
        ///     Locks a user out until the specified end date (set to a past date, to unlock a user)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns></returns>
        public Task SetLockoutEndDate(TUser user, DateTimeOffset lockoutEnd)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? (DateTime?)null : lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Used to record when an attempt to access the user has failed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> IncrementAccessFailedCount(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        ///     Used to reset the account access count, typically after the account is successfully accessed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task ResetAccessFailedCount(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Returns the current number of failed access attempts.  This number usually will be reset whenever the password is
        ///     verified or the account is locked out.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<int> GetAccessFailedCount(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        ///     Returns whether the user can be locked out.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabled(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        ///     Sets whether the user can be locked out.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetLockoutEnabled(TUser user, bool enabled)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Set the user's phone number
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task SetPhoneNumber(TUser user, string phoneNumber)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get a user's phone number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPhoneNumber(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        ///     Returns whether the user phoneNumber is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetPhoneNumberConfirmed(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        ///     Set PhoneNumberConfirmed on the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetPhoneNumberConfirmed(TUser user, bool confirmed)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Add a user to a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public virtual Task AddToRole(TUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            //if (String.IsNullOrWhiteSpace(roleName))
            //{
            //    throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "roleName");
            //}
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Remove a user from a role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public virtual Task RemoveFromRole(TUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            //if (String.IsNullOrWhiteSpace(roleName))
            //{
            //    throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "roleName");
            //}
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Get the names of the roles a user is a member of
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<IList<string>> GetRoles(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns true if the user is in the named role
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public virtual Task<bool> IsInRole(TUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            //if (String.IsNullOrWhiteSpace(roleName))
            //{
            //    throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "roleName");
            //}
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Set the security stamp for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Task SetSecurityStamp(TUser user, string stamp)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get the security stamp for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetSecurityStamp(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        ///     Set whether two factor authentication is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public Task SetTwoFactorEnabled(TUser user, bool enabled)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Gets whether two factor authentication is enabled for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetTwoFactorEnabled(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.TwoFactorEnabled);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     Dispose the store
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }
    }
}
