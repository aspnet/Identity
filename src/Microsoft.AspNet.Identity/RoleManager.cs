// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.Logging;
using Microsoft.AspNet.Identity.Store;
using Microsoft.Framework.Logging;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    /// Provides the APIs for managing roles in a persistence store.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a role.</typeparam>
    public class RoleManager<TRole> : IDisposable where TRole : class
    {
        private bool _disposed;
        private readonly HttpContext _context;
        private CancellationToken CancellationToken => _context?.RequestAborted ?? CancellationToken.None;

        /// <summary>
        /// Constructs a new instance of <see cref="RoleManager{TRole}"/>.
        /// </summary>
        /// <param name="store">The persistence store the manager will operate over.</param>
        /// <param name="roleValidators">A collection of validators for roles.</param>
        /// <param name="keyNormalizer">The normalizer to use when normalizing role names to keys.</param>
        /// <param name="errors">The <see cref="IdentityErrorDescriber"/> used to provider error messages.</param>
        /// <param name="logger">The logger used to log messages, warnings and errors.</param>
        /// <param name="contextAccessor">The accessor used to access the <see cref="HttpContext"/>.</param>
        public RoleManager(IRoleStore<TRole> store,
            IEnumerable<IRoleValidator<TRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<TRole>> logger,
            IHttpContextAccessor contextAccessor)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            Store = store;
            KeyNormalizer = keyNormalizer ?? new UpperInvariantLookupNormalizer();
            ErrorDescriber = errors ?? new IdentityErrorDescriber();
            _context = contextAccessor?.HttpContext;
            Logger = logger;

            if (roleValidators != null)
            {
                foreach (var v in roleValidators)
                {
                    RoleValidators.Add(v);
                }
            }
        }

        /// <summary>
        /// Gets the persistence store this instance operates over.
        /// </summary>
        /// <value>The persistence store this instance operates over.</value>
        protected IRoleStore<TRole> Store { get; private set; }

        /// <summary>
        /// Gets the <see cref="ILogger"/> used to log messages from the manager.
        /// </summary>
        /// <value>
        /// The <see cref="ILogger"/> used to log messages from the manager.
        /// </value>
        protected internal virtual ILogger Logger { get; set; }

        /// <summary>
        /// Gets a list of validators for roles to call before persistence.
        /// </summary>
        /// <value>A list of validators for roles to call before persistence.</value>
        internal IList<IRoleValidator<TRole>> RoleValidators { get; } = new List<IRoleValidator<TRole>>();

        /// <summary>
        /// Gets the <see cref="IdentityErrorDescriber"/> used to provider error messages.
        /// </summary>
        /// <value>
        /// The <see cref="IdentityErrorDescriber"/> used to provider error messages.
        /// </value>
        internal IdentityErrorDescriber ErrorDescriber { get; set; }

        /// <summary>
        /// Gets the normalizer to use when normalizing role names to keys.
        /// </summary>
        /// <value>
        /// The normalizer to use when normalizing role names to keys.
        /// </value>
        internal ILookupNormalizer KeyNormalizer { get; set; }

        /// <summary>
        /// Gets an IQueryable collection of Roles if the persistence store is an <see cref="IQueryableRoleStore"/>,
        /// otherwise throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <value>An IQueryable collection of Roles if the persistence store is an <see cref="IQueryableRoleStore"/>.</value>
        /// <exception cref="NotSupportedException">Thrown if the persistence store is not an <see cref="IQueryableRoleStore"/>.</exception>
        /// <remarks>
        /// Callers to this property should use <see cref="SupportsQueryableRoles"/> to ensure the backing role store supports 
        /// returning an IQueryable list of roles.
        /// </remarks>
        public virtual IQueryable<TRole> Roles
        {
            get
            {
                var queryableStore = Store as IQueryableRoleStore<TRole>;
                if (queryableStore == null)
                {
                    throw new NotSupportedException(Resources.StoreNotIQueryableRoleStore);
                }
                return queryableStore.Roles;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the underlying persistence store supports returning an <see cref="IQueryable"/> collection of roles.
        /// </summary>
        /// <value>
        /// true if the underlying persistence store supports returning an <see cref="IQueryable"/> collection of roles, otherwise false.
        /// </value>
        public virtual bool SupportsQueryableRoles
        {
            get
            {
                ThrowIfDisposed();
                return Store is IQueryableRoleStore<TRole>;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the underlying persistence store supports <see cref="Claim"/>s for roles.
        /// </summary>
        /// <value>
        /// true if the underlying persistence store supports <see cref="Claim"/>s for roles, otherwise false.
        /// </value>
        public virtual bool SupportsRoleClaims
        {
            get
            {
                ThrowIfDisposed();
                return Store is IRoleClaimStore<TRole>;
            }
        }

        /// <summary>
        /// Creates the specified <paramref name="role"/> in the persistence store, as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public virtual async Task<IdentityResult> CreateAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            var result = await ValidateRoleInternal(role);
            if (!result.Succeeded)
            {
                return result;
            }
            await UpdateNormalizedRoleNameAsync(role);
            result = await Store.CreateAsync(role, CancellationToken);
            using (await BeginLoggingScopeAsync(role))
            {
                return Logger.Log(result);
            }
        }

        /// <summary>
        /// Updates the normalized name for the specified <paramref name="role"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name needs to be updated.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public virtual async Task UpdateNormalizedRoleNameAsync(TRole role)
        {
            var name = await GetRoleNameAsync(role);
            await Store.SetNormalizedRoleNameAsync(role, NormalizeKey(name), CancellationToken);
        }

        /// <summary>
        /// Updates the specified <paramref name="role"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to updated.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> for the update.
        /// </returns>
        public virtual async Task<IdentityResult> UpdateAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            using (await BeginLoggingScopeAsync(role))
            {
                return Logger.Log(await UpdateRoleAsync(role));
            }
        }

        /// <summary>
        /// Deletes the specified <paramref name="role"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to delete.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> for the delete.
        /// </returns>
        public virtual async Task<IdentityResult> DeleteAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            using (await BeginLoggingScopeAsync(role))
            {
                return Logger.Log(await Store.DeleteAsync(role, CancellationToken));
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="roleName"/> exists, as an asynchronous operation.
        /// </summary>
        /// <param name="roleName">The role name whose existence should be checked.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing true if the role name exists, otherwise false.
        /// </returns>
        public virtual async Task<bool> RoleExistsAsync(string roleName)
        {
            ThrowIfDisposed();
            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            return await FindByNameAsync(NormalizeKey(roleName)) != null;
        }

        /// <summary>
        /// Gets a normalized representation of the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The value to normalize.</param>
        /// <returns>A normalized representation of the specified <paramref name="key"/>.</returns>
        public virtual string NormalizeKey(string key)
        {
            return (KeyNormalizer == null) ? key : KeyNormalizer.Normalize(key);
        }

        /// <summary>
        /// Finds the role associated with the specified <paramref name="roleId"/> if any, as an asynchronous operation.
        /// </summary>
        /// <param name="roleId">The role ID whose role should be returned.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the role 
        /// associated with the specified <paramref name="roleId"/>
        /// </returns>
        public virtual async Task<TRole> FindByIdAsync(string roleId)
        {
            ThrowIfDisposed();
            return await Store.FindByIdAsync(roleId, CancellationToken);
        }

        /// <summary>
        /// Gets the name of the specified <paramref name="role"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be retrieved.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the name of the 
        /// specified <paramref name="role"/>.
        /// </returns>
        public virtual async Task<string> GetRoleNameAsync(TRole role)
        {
            ThrowIfDisposed();
            return await Store.GetRoleNameAsync(role, CancellationToken);
        }

        /// <summary>
        /// Sets the name of the specified <paramref name="role"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be set.</param>
        /// <param name="name">The name to set.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public virtual async Task<IdentityResult> SetRoleNameAsync(TRole role, string name)
        {
            ThrowIfDisposed();

            using (await BeginLoggingScopeAsync(role))
            {
                await Store.SetRoleNameAsync(role, name, CancellationToken);
                await UpdateNormalizedRoleNameAsync(role);
                return Logger.Log(IdentityResult.Success);
            }
        }

        /// <summary>
        /// Gets the ID of the specified <paramref name="role"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose ID should be retrieved.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the ID of the 
        /// specified <paramref name="role"/>.
        /// </returns>
        public virtual async Task<string> GetRoleIdAsync(TRole role)
        {
            ThrowIfDisposed();
            return await Store.GetRoleIdAsync(role, CancellationToken);
        }

        /// <summary>
        /// Finds the role associated with the specified <paramref name="roleName"/> if any, as an asynchronous operation.
        /// </summary>
        /// <param name="roleName">The name of the role to be returned.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the role 
        /// associated with the specified <paramref name="roleName"/>
        /// </returns>
        public virtual async Task<TRole> FindByNameAsync(string roleName)
        {
            ThrowIfDisposed();
            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            return await Store.FindByNameAsync(NormalizeKey(roleName), CancellationToken);
        }

        /// <summary>
        /// Adds a claim to a role, as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to add the claim to.</param>
        /// <param name="claim">The claim to add.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public virtual async Task<IdentityResult> AddClaimAsync(TRole role, Claim claim)
        {
            ThrowIfDisposed();
            var claimStore = GetClaimStore();
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            using (await BeginLoggingScopeAsync(role))
            {
                await claimStore.AddClaimAsync(role, claim, CancellationToken);
                return Logger.Log(await UpdateRoleAsync(role));
            }
        }

        /// <summary>
        /// Removes a claim from a role, as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to remove the claim from.</param>
        /// <param name="claim">The claim to add.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public virtual async Task<IdentityResult> RemoveClaimAsync(TRole role, Claim claim)
        {
            ThrowIfDisposed();
            var claimStore = GetClaimStore();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            using (await BeginLoggingScopeAsync(role))
            {
                await claimStore.RemoveClaimAsync(role, claim, CancellationToken);
                return Logger.Log(await UpdateRoleAsync(role));
            }
        }

        /// <summary>
        /// Gets a list of claims associated with the specified <paramref name="role"/>, as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose claims should be returned.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the list of <see cref="Claim"/>s
        /// associated with the specified <paramref name="role"/>.
        /// </returns>
        public virtual async Task<IList<Claim>> GetClaimsAsync(TRole role)
        {
            ThrowIfDisposed();
            var claimStore = GetClaimStore();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            return await claimStore.GetClaimsAsync(role, CancellationToken);
        }

        /// <summary>
        /// Releases all resources used by the role manager.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the role manager and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                Store.Dispose();
            }
            _disposed = true;
        }

        protected virtual async Task<IDisposable> BeginLoggingScopeAsync(TRole role, [CallerMemberName] string methodName = null)
        {
            var state = Resources.FormatLoggingResultMessageForRole(methodName, await GetRoleIdAsync(role));
            return Logger?.BeginScope(state);
        }

        private async Task<IdentityResult> ValidateRoleInternal(TRole role)
        {
            var errors = new List<IdentityError>();
            foreach (var v in RoleValidators)
            {
                var result = await v.ValidateAsync(this, role);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors);
                }
            }
            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }

        private async Task<IdentityResult> UpdateRoleAsync(TRole role)
        {
            var result = await ValidateRoleInternal(role);
            if (!result.Succeeded)
            {
                return result;
            }
            await UpdateNormalizedRoleNameAsync(role);
            return await Store.UpdateAsync(role, CancellationToken);
        }

        // IRoleClaimStore methods
        private IRoleClaimStore<TRole> GetClaimStore()
        {
            var cast = Store as IRoleClaimStore<TRole>;
            if (cast == null)
            {
                throw new NotSupportedException(Resources.StoreNotIRoleClaimStore);
            }
            return cast;
        }
            
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}