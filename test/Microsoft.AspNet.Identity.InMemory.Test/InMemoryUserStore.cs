// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity.InMemory
{
    public class InMemoryUserStore<TUser> :
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser>
        where TUser : IdentityUser
    {
        private readonly Dictionary<UserLoginInfo, TUser> _logins =
            new Dictionary<UserLoginInfo, TUser>(new LoginComparer());

        private readonly Dictionary<string, TUser> _users = new Dictionary<string, TUser>();

        public IQueryable<TUser> Users
        {
            get { return _users.Values.AsQueryable(); }
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            var claims = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            return Task.FromResult<IList<Claim>>(claims);
        }

        public Task AddClaimAsync(TUser user, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.Claims.Add(new IdentityUserClaim<string> { ClaimType = claim.Type, ClaimValue = claim.Value, UserId = user.Id });
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entity =
                user.Claims.FirstOrDefault(
                    uc => uc.UserId == user.Id && uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value);
            if (entity != null)
            {
                user.Claims.Remove(entity);
            }
            return Task.FromResult(0);
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<TUser> FindByEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            return
                Task.FromResult(
                    Users.FirstOrDefault(u => String.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.LockoutEnd = lockoutEnd;
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.Logins.Add(new IdentityUserLogin<string>
            {
                UserId = user.Id, 
                LoginProvider = login.LoginProvider, 
                ProviderKey = login.ProviderKey
            });
            _logins[login] = user;
            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            var loginEntity =
                user.Logins.SingleOrDefault(
                    l =>
                        l.ProviderKey == login.ProviderKey && l.LoginProvider == login.LoginProvider &&
                        l.UserId == user.Id);
            if (loginEntity != null)
            {
                user.Logins.Remove(loginEntity);
            }
            _logins[login] = null;
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            var logins = user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList();
            return Task.FromResult<IList<UserLoginInfo>>(logins);
        }

        public Task<TUser> FindByLoginAsync(UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_logins.ContainsKey(login))
            {
                return Task.FromResult(_logins[login]);
            }
            return Task.FromResult<TUser>(null);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = new CancellationToken())
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public Task CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            _users[user.Id] = user;
            return Task.FromResult(0);
        }

        public Task UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            _users[user.Id] = user;
            return Task.FromResult(0);
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_users.ContainsKey(userId))
            {
                return Task.FromResult(_users[userId]);
            }
            return Task.FromResult<TUser>(null);
        }

        public void Dispose()
        {
        }

        public Task<TUser> FindByNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return
                Task.FromResult(
                    Users.FirstOrDefault(u => String.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase)));
        }

        public Task DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null || !_users.ContainsKey(user.Id))
            {
                throw new InvalidOperationException("Unknown user");
            }
            _users.Remove(user.Id);
            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        // RoleId == roleName for InMemory
        public Task AddToRoleAsync(TUser user, string role, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.Roles.Add(new IdentityUserRole<string> { RoleId = role, UserId = user.Id });
            return Task.FromResult(0);
        }

        // RoleId == roleName for InMemory
        public Task RemoveFromRoleAsync(TUser user, string role, CancellationToken cancellationToken = default(CancellationToken))
        {
            var roleEntity = user.Roles.SingleOrDefault(ur => ur.RoleId == role);
            if (roleEntity != null)
            {
                user.Roles.Remove(roleEntity);
            }
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<IList<string>>(user.Roles.Select(ur => ur.RoleId).ToList());
        }

        public Task<bool> IsInRoleAsync(TUser user, string role, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.Roles.Any(ur => ur.RoleId == role));
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        private class LoginComparer : IEqualityComparer<UserLoginInfo>
        {
            public bool Equals(UserLoginInfo x, UserLoginInfo y)
            {
                return x.LoginProvider == y.LoginProvider && x.ProviderKey == y.ProviderKey;
            }

            public int GetHashCode(UserLoginInfo obj)
            {
                return (obj.ProviderKey + "--" + obj.LoginProvider).GetHashCode();
            }
        }
    }
}