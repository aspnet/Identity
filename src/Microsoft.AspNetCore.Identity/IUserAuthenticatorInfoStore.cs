// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Provides an abstraction for a store which stores info about user's authenticator.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    public interface IUserAuthenticatorInfoStore<TUser> : IUserStore<TUser> where TUser : class
    {
        /// <summary>
        /// Resets the authenticator key for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose authenticator key should be set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task ResetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken);

        /// <summary>
        /// Get the authenticator key for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user"/>.</returns>
        Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken);

        /// <summary>
        /// Generates recovery codes for the user, this invalidates any previous recovery codes for the user.
        /// </summary>
        /// <param name="user">The user to generate recovery codes for.</param>
        /// <param name="number">The number of codes to generate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The new recovery codes for the user.</returns>
        Task<List<string>> GenerateNewRecoveryCodesAsync(TUser user, int number, CancellationToken cancellationToken);

        /// <summary>
        /// Returns whether a recovery code is valid for a user. Note: recovery codes are only valid
        /// once, and will be invalid after use.
        /// </summary>
        /// <param name="user">The user who owns the recovery code.</param>
        /// <param name="code">The recovery code to use.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>True if the recovery code was found for the user.</returns>
        Task<bool> UseRecoveryCodeAsync(TUser user, string code, CancellationToken cancellationToken);
    }
}