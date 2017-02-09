// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Provides an abstraction for a store containing users' password expiration data.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    public interface IUserPasswordExpirationStore<TUser> : IUserStore<TUser> where TUser : class
    {
        /// <summary>
        /// Sets the last password change date.for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose password hash to set.</param>
        /// <param name="changeDate">The last password change date.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SetLastPasswordChangeDateAsync(TUser user, DateTimeOffset changeDate, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the last password change date for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose password hash to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, returning the password hash for the specified <paramref name="user"/>.</returns>
        Task<string> GetLastPasswordChangeDateAsync(TUser user, CancellationToken cancellationToken);
    }
}