// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity.Store
{
    /// <summary>
    /// Provides an abstraction to store a flag indicating whether a user has two factor authentication enabled.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    public interface IUserTwoFactorStore<TUser> : IUserStore<TUser> where TUser : class
    {
        /// <summary>
        /// Sets a flag indicating whether the specified <paramref name="user "/>has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="enabled">A flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user "/>has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified 
        /// <paramref name="user "/>has two factor authentication enabled or not.
        /// </returns>
        Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken);
    }
}