// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public abstract class ClientStoreBase<TClient, TScope, TClientClaim, TRedirectUri, TContext, TUserKey>
        : IClientRedirectUriStore<TClient>,
        IClientClaimStore<TClient>,
        IClientSecretStore<TClient>,
        IClientScopeStore<TClient>,
        IQueryableClientStore<TClient>
        where TClient : IdentityClient<TUserKey, TScope, TClientClaim, TRedirectUri>
        where TScope : IdentityClientScope, new()
        where TClientClaim : IdentityClientClaim, new()
        where TRedirectUri : IdentityClientRedirectUri, new()
        where TUserKey : IEquatable<TUserKey>
    {
        private bool _disposed;

        public ClientStoreBase(IdentityErrorDescriber describer)
        {
            if (describer == null)
            {
                throw new ArgumentNullException(nameof(describer));
            }

            ErrorDescriber = describer;
        }

        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public abstract IQueryable<TClient> Clients { get; }

        public abstract Task<IdentityResult> CreateAsync(TClient client, CancellationToken cancellationToken);

        public abstract Task<IdentityResult> UpdateAsync(TClient client, CancellationToken cancellationToken);

        public abstract Task<IdentityResult> DeleteAsync(TClient client, CancellationToken cancellationToken);

        public void Dispose()
        {
            _disposed = true;
        }

        public abstract Task<TClient> FindByClientIdAsync(string clientId, CancellationToken cancellationToken);

        public abstract Task<TClient> FindByNameAsync(string name, CancellationToken cancellationToken);

        public abstract Task<IEnumerable<TClient>> FindByUserIdAsync(string userId, CancellationToken cancellationToken);

        public abstract Task<TClient> FindByIdAsync(string clientId, CancellationToken cancellationToken);

        public virtual Task<string> GetIdAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Task.FromResult(client.Id);
        }

        public virtual Task<string> GetUserIdAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var id = ConvertUserIdToString(client.UserId);
            return Task.FromResult(id);
        }

        public virtual Task<string> GetNameAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Task.FromResult(client.Name);
        }

        public abstract Task<IEnumerable<string>> FindRegisteredUrisAsync(TClient client, CancellationToken cancellationToken);

        public abstract Task<IdentityResult> RegisterRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken);

        public virtual TRedirectUri CreateRedirectUri(TClient client, string redirectUri, bool isLogout)
            => new TRedirectUri
            {
                ClientId = client.Id,
                IsLogout = isLogout,
                Value = redirectUri
            };

        public abstract Task<IdentityResult> UnregisterRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken);

        public abstract Task<IdentityResult> UpdateRedirectUriAsync(TClient client, string oldRedirectUri, string newRedirectUri, CancellationToken cancellationToken);

        public abstract Task<IEnumerable<string>> FindRegisteredLogoutUrisAsync(TClient client, CancellationToken cancellationToken);

        public abstract Task<IdentityResult> RegisterLogoutRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken);

        public abstract Task<IdentityResult> UnregisterLogoutRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken);

        public abstract Task<IdentityResult> UpdateLogoutRedirectUriAsync(TClient client, string oldRedirectUri, string newRedirectUri, CancellationToken cancellationToken);

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public virtual string ConvertUserIdToString(TUserKey id)
        {
            if (Equals(id, default(TUserKey)))
            {
                return null;
            }
            return id.ToString();
        }

        public virtual TUserKey ConvertUserIdFromString(string id)
        {
            if (id == null)
            {
                return default(TUserKey);
            }
            return (TUserKey)TypeDescriptor.GetConverter(typeof(TUserKey)).ConvertFromInvariantString(id);
        }

        public virtual Task SetClientSecretHashAsync(TClient client, string clientSecretHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            client.ClientSecretHash = clientSecretHash;
            return Task.CompletedTask;
        }

        public Task<string> GetClientSecretHashAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return Task.FromResult(client.ClientSecretHash);
        }

        public virtual Task<bool> HasClientSecretAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return Task.FromResult(client.ClientSecretHash != null);
        }

        public abstract Task<IEnumerable<string>> FindScopesAsync(TClient client, CancellationToken cancellationToken);

        public abstract Task<IdentityResult> AddScopeAsync(TClient client, string scope, CancellationToken cancellationToken);

        public virtual TScope CreateScope(TClient client, string scope)
        {
            return new TScope
            {
                ClientId = client.Id,
                Value = scope
            };
        }

        public abstract Task<IdentityResult> UpdateScopeAsync(TClient client, string oldScope, string newScope, CancellationToken cancellationToken);

        public abstract Task<IdentityResult> RemoveScopeAsync(TClient client, string scope, CancellationToken cancellationToken);

        public abstract Task<IList<Claim>> GetClaimsAsync(TClient application, CancellationToken cancellationToken);

        public abstract Task AddClaimsAsync(TClient client, IEnumerable<Claim> claims, CancellationToken cancellationToken);

        public virtual TClientClaim CreateClientClaim(TClient client, Claim claim) =>
            new TClientClaim
            {
                ClientId = client.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };

        public abstract Task ReplaceClaimAsync(TClient client, Claim oldClaim, Claim newClaim, CancellationToken cancellationToken);

        public abstract Task RemoveClaimsAsync(TClient client, IEnumerable<Claim> claims, CancellationToken cancellationToken);
    }
}