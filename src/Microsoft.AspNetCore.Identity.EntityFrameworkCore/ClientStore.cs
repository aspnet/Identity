// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.AspNetCore.Identity
{
    public class ClientStore<TClient, TScope, TClientClaim, TRedirectUri, TContext, TUserKey>
        : ClientStoreBase<TClient, TScope, TClientClaim, TRedirectUri, TContext, TUserKey>
        where TClient : IdentityClient<TUserKey, TScope, TClientClaim, TRedirectUri>
        where TScope : IdentityClientScope, new()
        where TClientClaim : IdentityClientClaim, new()
        where TRedirectUri : IdentityClientRedirectUri, new()
        where TContext : DbContext
        where TUserKey : IEquatable<TUserKey>
    {
        public ClientStore(TContext context, IdentityErrorDescriber describer) : base(describer)
            => Context = context;

        public TContext Context { get; }

        public DbSet<TClient> ClientsSet => Context.Set<TClient>();

        public DbSet<TScope> Scopes => Context.Set<TScope>();

        public DbSet<TClientClaim> ClientClaims => Context.Set<TClientClaim>();

        public DbSet<TRedirectUri> RedirectUris => Context.Set<TRedirectUri>();

        public override IQueryable<TClient> Clients => ClientsSet;

        public bool AutoSaveChanges { get; set; } = true;

        protected Task SaveChanges(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
        }

        public override async Task<IdentityResult> CreateAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            Context.Add(client);
            await SaveChanges(cancellationToken);
            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UpdateAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            Context.Attach(client);
            client.ConcurrencyStamp = Guid.NewGuid().ToString();
            Context.Update(client);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> DeleteAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            Context.Remove(client);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        public override Task<TClient> FindByClientIdAsync(string clientId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Clients.SingleOrDefaultAsync(c => c.Id == clientId, cancellationToken);
        }

        public override Task<TClient> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Clients.SingleOrDefaultAsync(c => c.Name == name, cancellationToken);
        }

        public override async Task<IEnumerable<TClient>> FindByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var id = ConvertUserIdFromString(userId);
            return await Clients.Where(c => c.UserId.Equals(id)).ToListAsync();
        }

        public override Task<TClient> FindByIdAsync(string ClientId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var id = ConvertUserIdFromString(ClientId);
            return ClientsSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public override async Task<IEnumerable<string>> FindRegisteredUrisAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var redirectUris = await RedirectUris
                .Where(ru => ru.ClientId.Equals(client.Id) && !ru.IsLogout)
                .Select(ru => ru.Value)
                .ToListAsync(cancellationToken);

            return redirectUris;
        }

        public override async Task<IdentityResult> RegisterRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (redirectUri == null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }

            var existingRedirectUri = await RedirectUris.SingleOrDefaultAsync(
                ru => ru.ClientId.Equals(client.Id) && ru.Value.Equals(redirectUri) && !ru.IsLogout);
            if (existingRedirectUri != null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "A route with the same value already exists." });
            }

            RedirectUris.Add(CreateRedirectUri(client, redirectUri, isLogout: false));

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UnregisterRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (redirectUri == null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }

            var registeredUri = await RedirectUris
                .SingleOrDefaultAsync(ru => ru.ClientId.Equals(client.Id) && ru.Value.Equals(redirectUri) && !ru.IsLogout);
            if (registeredUri == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "We were unable to find the redirect uri to unregister." });
            }

            RedirectUris.Remove(registeredUri);

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UpdateRedirectUriAsync(TClient client, string oldRedirectUri, string newRedirectUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (oldRedirectUri == null)
            {
                throw new ArgumentNullException(nameof(oldRedirectUri));
            }

            if (newRedirectUri == null)
            {
                throw new ArgumentNullException(nameof(newRedirectUri));
            }

            var existingRedirectUri = await RedirectUris
                .SingleOrDefaultAsync(ru => ru.ClientId.Equals(client.Id) && ru.Value.Equals(oldRedirectUri) && !ru.IsLogout);

            if (existingRedirectUri == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "We were unable to find the registered redirect uri to update." });
            }

            existingRedirectUri.Value = newRedirectUri;

            return IdentityResult.Success;
        }

        public override async Task<IEnumerable<string>> FindRegisteredLogoutUrisAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var redirectUris = await RedirectUris
                .Where(ru => ru.ClientId.Equals(client.Id) && ru.IsLogout)
                .Select(ru => ru.Value)
                .ToListAsync(cancellationToken);

            return redirectUris;
        }

        public override async Task<IdentityResult> RegisterLogoutRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (redirectUri == null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }

            var existingRedirectUri = await RedirectUris.SingleOrDefaultAsync(
                ru => ru.ClientId.Equals(client.Id) && ru.Value.Equals(redirectUri) && ru.IsLogout);
            if (existingRedirectUri != null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "A route with the same value already exists." });
            }

            RedirectUris.Add(CreateRedirectUri(client, redirectUri, isLogout: true));

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UnregisterLogoutRedirectUriAsync(TClient client, string redirectUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (redirectUri == null)
            {
                throw new ArgumentNullException(nameof(redirectUri));
            }

            var registeredUri = await RedirectUris
                .SingleOrDefaultAsync(ru => ru.ClientId.Equals(client.Id) && ru.Value.Equals(redirectUri) && ru.IsLogout);
            if (registeredUri == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "We were unable to find the redirect uri to unregister." });
            }

            RedirectUris.Remove(registeredUri);

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UpdateLogoutRedirectUriAsync(TClient client, string oldRedirectUri, string newRedirectUri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (oldRedirectUri == null)
            {
                throw new ArgumentNullException(nameof(oldRedirectUri));
            }

            if (newRedirectUri == null)
            {
                throw new ArgumentNullException(nameof(newRedirectUri));
            }

            var existingRedirectUri = await RedirectUris
                .SingleOrDefaultAsync(ru => ru.ClientId.Equals(client.Id) && ru.Value.Equals(oldRedirectUri) && ru.IsLogout);

            if (existingRedirectUri == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "We were unable to find the registered redirect uri to update." });
            }

            existingRedirectUri.Value = newRedirectUri;

            return IdentityResult.Success;
        }

        public override async Task<IEnumerable<string>> FindScopesAsync(TClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var scopes = await Scopes
                .Where(s => s.ClientId.Equals(client.Id))
                .Select(s => s.Value)
                .ToListAsync(cancellationToken);

            return scopes;
        }

        public override async Task<IdentityResult> AddScopeAsync(TClient client, string scope, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            var existingScope = await Scopes.SingleOrDefaultAsync(
                ru => ru.ClientId.Equals(client.Id) && ru.Value.Equals(scope));
            if (existingScope != null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "A scope with the same value already exists." });
            }

            Scopes.Add(CreateScope(client, scope));
            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UpdateScopeAsync(TClient client, string oldScope, string newScope, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (oldScope == null)
            {
                throw new ArgumentNullException(nameof(oldScope));
            }

            if (newScope == null)
            {
                throw new ArgumentNullException(nameof(newScope));
            }

            var existingScope = await Scopes
                .SingleOrDefaultAsync(s => s.ClientId.Equals(client.Id) && s.Value.Equals(oldScope));
            if (existingScope == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "We were unable to find the scope to update." });
            }

            existingScope.Value = newScope;
            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> RemoveScopeAsync(TClient client, string scope, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            var existingScope = await Scopes
                .SingleOrDefaultAsync(ru => ru.ClientId.Equals(client.Id) && ru.Value.Equals(scope));
            if (existingScope == null)
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "We were unable to find the scope to remove." });
            }

            Scopes.Remove(existingScope);
            return IdentityResult.Success;
        }

        public override async Task<IList<Claim>> GetClaimsAsync(TClient application, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return await ClientClaims.Where(ac => ac.ClientId.Equals(application.Id)).Select(c => c.ToClaim()).ToListAsync(cancellationToken);
        }

        public override Task AddClaimsAsync(TClient client, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
            foreach (var claim in claims)
            {
                ClientClaims.Add(CreateClientClaim(client, claim));
            }
            return Task.CompletedTask;
        }

        public override async Task ReplaceClaimAsync(TClient client, Claim oldClaim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (oldClaim == null)
            {
                throw new ArgumentNullException(nameof(oldClaim));
            }
            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            var matchedClaims = await ClientClaims.Where(ac => ac.ClientId.Equals(client.Id) && ac.ClaimValue == oldClaim.Value && ac.ClaimType == oldClaim.Type).ToListAsync(cancellationToken);
            foreach (var matchedClaim in matchedClaims)
            {
                matchedClaim.ClaimValue = newClaim.Value;
                matchedClaim.ClaimType = newClaim.Type;
            }
        }

        public override async Task RemoveClaimsAsync(TClient client, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
            foreach (var claim in claims)
            {
                var matchedClaims = await ClientClaims.Where(ac => ac.ClientId.Equals(client.Id) && ac.ClaimValue == claim.Value && ac.ClaimType == claim.Type).ToListAsync(cancellationToken);
                foreach (var c in matchedClaims)
                {
                    ClientClaims.Remove(c);
                }
            }
        }
    }
}
