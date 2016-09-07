// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Provides an abstraction to store a user's authentication tokens.
    /// </summary>
    /// <typeparam name="TUser">The type encapsulating a user.</typeparam>
    public interface IUserTokenStore<TUser> : IUserStore<TUser> where TUser : class
    {
        /// <summary>
        /// Stores tokens for a particular user. Any tokens with an id that already exists will be replaced.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="tokens">The tokens to store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task StoreTokensAsync(TUser user, IEnumerable<IdentityToken> tokens, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes tokens for a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="ids">The unique identifiers for the tokens to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteTokensAsync(TUser user, IEnumerable<string> ids, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the token value.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="id">The unique token identifier.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<string> GetTokenAsync(TUser user, string id, CancellationToken cancellationToken);

        /// <summary>
        /// Returns all of a users tokens with the specified type.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="type">The type of tokens to return.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<IEnumerable<IdentityToken>> GetTokensAsync(TUser user, string type, CancellationToken cancellationToken);

        /// <summary>
        /// Returns all of a users tokens.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<IEnumerable<IdentityToken>> GetTokensAsync(TUser user, CancellationToken cancellationToken);
    }
}
