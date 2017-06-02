// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Identity
{
    ///
    public class ClientManager<TClient> : IDisposable where TClient : class
    {
        private bool _disposed;

        public ClientManager(
            IClientStore<TClient> store,
            IPasswordHasher<TClient> passwordHasher,
            IEnumerable<IClientValidator<TClient>> clientValidators,
            ILogger<ClientManager<TClient>> logger)
        {
            Store = store;
            PasswordHasher = passwordHasher;
            ClientValidators = clientValidators;
            Logger = Logger;
        }

        public IClientStore<TClient> Store { get; set; }
        public IPasswordHasher<TClient> PasswordHasher { get; set; }
        public IEnumerable<IClientValidator<TClient>> ClientValidators { get; set; }
        public ILogger<ClientManager<TClient>> Logger { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public virtual bool SupportsQueryableClients
        {
            get
            {
                ThrowIfDisposed();
                return Store is IQueryableUserStore<TClient>;
            }
        }

        public virtual IQueryable<TClient> Clients
        {
            get
            {
                var queryableStore = Store as IQueryableClientStore<TClient>;
                if (queryableStore == null)
                {
                    throw new NotSupportedException("Store not IQueryableClientStore");
                }
                return queryableStore.Clients;
            }
        }

        public Task<TClient> FindByIdAsync(string ClientId)
        {
            return Store.FindByIdAsync(ClientId, CancellationToken);
        }

        public Task<TClient> FindByNameAsync(string name)
        {
            return Store.FindByNameAsync(name, CancellationToken.None);
        }

        public virtual async Task<IdentityResult> CreateAsync(TClient client)
        {
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var result = await ValidateClientAsync(client);
            if (!result.Succeeded)
            {
                return result;
            }

            return await Store.CreateAsync(client, CancellationToken);
        }

        public virtual async Task<IdentityResult> DeleteAsync(TClient client)
        {
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return await Store.DeleteAsync(client, CancellationToken);
        }

        public virtual async Task<IdentityResult> UpdateAsync(TClient client)
        {
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var result = await ValidateClientAsync(client);
            if (!result.Succeeded)
            {
                return result;
            }

            return await Store.UpdateAsync(client, CancellationToken);
        }

        private async Task<IdentityResult> ValidateClientAsync(TClient client)
        {
            var errors = new List<IdentityError>();
            foreach (var v in ClientValidators)
            {
                var result = await v.ValidateAsync(this, client);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors);
                }
            }

            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            return IdentityResult.Success;
        }

        public Task<string> GetClientIdAsync(TClient client)
        {
            ThrowIfDisposed();
            return Store.GetIdAsync(client, CancellationToken);
        }

        public void Dispose()
        {
            _disposed = true;
        }

        public Task<string> GenerateClientSecretAsync()
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

        public async Task<IdentityResult> AddClientSecretAsync(TClient client, string clientSecret)
        {
            ThrowIfDisposed();
            var store = GetClientSecretStore();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var hash = await store.GetClientSecretHashAsync(client, CancellationToken);
            if (hash != null)
            {
                Logger.LogWarning(1, "User {clientId} already has a password.", await GetClientIdAsync(client));
                return IdentityResult.Failed();
            }
            var result = await UpdateClientSecretHashAsync(store, client, clientSecret);
            if (!result.Succeeded)
            {
                return result;
            }

            return await UpdateAsync(client);
        }

        public async Task<IdentityResult> ChangeClientSecretAsync(TClient client, string newClientSecret)
        {
            ThrowIfDisposed();
            var store = GetClientSecretStore();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var result = await UpdateClientSecretHashAsync(store, client, newClientSecret);
            if (!result.Succeeded)
            {
                return result;
            }

            return await UpdateAsync(client);
        }

        public async Task<IdentityResult> RemoveClientSecretAsync(TClient client)
        {
            ThrowIfDisposed();
            var store = GetClientSecretStore();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var result = await UpdateClientSecretHashAsync(store, client, clientSecret: null);
            if (!result.Succeeded)
            {
                return result;
            }

            return await UpdateAsync(client);
        }

        private IClientRedirectUriStore<TClient> GetRedirectUriStore()
        {
            if (Store is IClientRedirectUriStore<TClient> cast)
            {
                return cast;
            }

            throw new NotSupportedException();
        }

        public async Task<IdentityResult> RegisterRedirectUriAsync(TClient client, string redirectUri)
        {
            var redirectStore = GetRedirectUriStore();
            var result = await redirectStore.RegisterRedirectUriAsync(client, redirectUri, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return await redirectStore.UpdateAsync(client, CancellationToken);
        }

        public Task<IEnumerable<string>> FindRegisteredUrisAsync(TClient client)
        {
            var redirectStore = GetRedirectUriStore();
            return redirectStore.FindRegisteredUrisAsync(client, CancellationToken);
        }

        public Task<IEnumerable<string>> FindRegisteredLogoutUrisAsync(TClient client)
        {
            var redirectStore = GetRedirectUriStore();
            return redirectStore.FindRegisteredLogoutUrisAsync(client, CancellationToken);
        }

        public async Task<IdentityResult> UnregisterRedirectUriAsync(TClient client, string redirectUri)
        {
            var redirectStore = GetRedirectUriStore();
            var result = await redirectStore.UnregisterRedirectUriAsync(client, redirectUri, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return await redirectStore.UpdateAsync(client, CancellationToken);
        }

        public async Task<IdentityResult> UpdateRedirectUriAsync(TClient client, string oldRedirectUri, string newRedirectUri)
        {
            var redirectStore = GetRedirectUriStore();
            var result = await redirectStore.UpdateRedirectUriAsync(client, oldRedirectUri, newRedirectUri, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return await redirectStore.UpdateAsync(client, CancellationToken);
        }

        public async Task<bool> ValidateClientCredentialsAsync(string clientId, string clientSecret)
        {
            var client = await FindByIdAsync(clientId);
            if (client == null)
            {
                return false;
            }

            var clientSecretStore = GetClientSecretStore();
            if (!await clientSecretStore.HasClientSecretAsync(client, CancellationToken))
            {
                // Should we fail if clientSecret != null?
                return true;
            }

            if (clientSecret == null)
            {
                return false;
            }

            var result = await VerifyClientSecretAsync(clientSecretStore, client, clientSecret);
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                await UpdateClientSecretHashAsync(clientSecretStore, client, clientSecret);
                await UpdateAsync(client);
                return true;
            }

            return result == PasswordVerificationResult.Success;
        }

        private async Task<IdentityResult> UpdateClientSecretHashAsync(
            IClientSecretStore<TClient> clientSecretStore,
            TClient client,
            string clientSecret)
        {
            var hash = PasswordHasher.HashPassword(client, clientSecret);
            await clientSecretStore.SetClientSecretHashAsync(client, hash, CancellationToken);
            return IdentityResult.Success;
        }

        protected virtual async Task<PasswordVerificationResult> VerifyClientSecretAsync(
            IClientSecretStore<TClient> store,
            TClient client,
            string clientSecret)
        {
            var hash = await store.GetClientSecretHashAsync(client, CancellationToken);
            if (hash == null)
            {
                return PasswordVerificationResult.Failed;
            }

            return PasswordHasher.VerifyHashedPassword(client, hash, clientSecret);
        }

        private IClientSecretStore<TClient> GetClientSecretStore()
        {
            if (Store is IClientSecretStore<TClient> cast)
            {
                return cast;
            }

            throw new NotSupportedException();
        }

        public Task<IEnumerable<string>> FindScopesAsync(TClient client)
        {
            var scopeStore = GetScopeStore();
            return scopeStore.FindScopesAsync(client, CancellationToken);
        }

        public async Task<IdentityResult> AddScopeAsync(TClient client, string scope)
        {
            var scopeStore = GetScopeStore();
            var result = await scopeStore.AddScopeAsync(client, scope, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return await scopeStore.UpdateAsync(client, CancellationToken);
        }

        public async Task<IdentityResult> RemoveScopeAsync(TClient client, string scope)
        {
            var scopeStore = GetScopeStore();
            var result = await scopeStore.RemoveScopeAsync(client, scope, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return await scopeStore.UpdateAsync(client, CancellationToken);
        }

        public async Task<IdentityResult> UpdateScopeAsync(TClient client, string oldScope, string newScope)
        {
            var scopeStore = GetScopeStore();
            var result = await scopeStore.UpdateScopeAsync(client, oldScope, newScope, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return await scopeStore.UpdateAsync(client, CancellationToken);
        }

        private IClientScopeStore<TClient> GetScopeStore()
        {
            if (Store is IClientScopeStore<TClient> cast)
            {
                return cast;
            }

            throw new NotSupportedException();
        }

        public virtual Task<IdentityResult> AddClaimAsync(TClient client, Claim claim)
        {
            ThrowIfDisposed();
            var claimStore = GetClientClaimStore();
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return AddClaimsAsync(client, new Claim[] { claim });
        }

        public virtual async Task<IdentityResult> AddClaimsAsync(TClient client, IEnumerable<Claim> claims)
        {
            ThrowIfDisposed();
            var claimStore = GetClientClaimStore();
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            await claimStore.AddClaimsAsync(client, claims, CancellationToken);
            return await UpdateAsync(client);
        }

        public virtual async Task<IdentityResult> ReplaceClaimAsync(TClient client, Claim claim, Claim newClaim)
        {
            ThrowIfDisposed();
            var claimStore = GetClientClaimStore();
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            await claimStore.ReplaceClaimAsync(client, claim, newClaim, CancellationToken);
            return await UpdateAsync(client);
        }

        public virtual Task<IdentityResult> RemoveClaimAsync(TClient client, Claim claim)
        {
            ThrowIfDisposed();
            var claimStore = GetClientClaimStore();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            return RemoveClaimsAsync(client, new Claim[] { claim });
        }

        public virtual async Task<IdentityResult> RemoveClaimsAsync(TClient client, IEnumerable<Claim> claims)
        {
            ThrowIfDisposed();
            var claimStore = GetClientClaimStore();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            await claimStore.RemoveClaimsAsync(client, claims, CancellationToken);
            return await UpdateAsync(client);
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(TClient client)
        {
            ThrowIfDisposed();
            var claimStore = GetClientClaimStore();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            return await claimStore.GetClaimsAsync(client, CancellationToken);
        }

        private IClientClaimStore<TClient> GetClientClaimStore()
        {
            if (Store is IClientClaimStore<TClient> cast)
            {
                return cast;
            }

            throw new NotSupportedException();
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
