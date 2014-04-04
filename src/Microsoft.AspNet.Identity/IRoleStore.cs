﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Interface that exposes basic role management
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    public interface IRoleStore<TRole> : IDisposable where TRole : class
    {
        /// <summary>
        ///     Insert a new role
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CreateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     UpdateAsync a role
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     DeleteAsync a role
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Returns a role's id
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Returns a role's name
        /// </summary>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Finds a role by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     FindByLoginAsync a role by name
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TRole> FindByNameAsync(string roleName, CancellationToken cancellationToken = default(CancellationToken));
    }
}