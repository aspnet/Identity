//// Copyright (c) .NET Foundation. All rights reserved.
//// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using System.Threading;
//using System.Threading.Tasks;

//namespace Microsoft.AspNetCore.Identity
//{
//    /// <summary>
//    /// Provides an abstraction to store tokens.
//    /// </summary>
//    public interface ITokenStore
//    {
//        /// <summary>
//        /// Sets the token value for a particular user.
//        /// </summary>
//        /// <param name="id">The id used to identify the token.</param>
//        /// <param name="value">The type of the token.</param>
//        /// <param name="value">The value of the token.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
//        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
//        Task CreateAsync(string id, string type, string value, CancellationToken cancellationToken);

//        /// <summary>
//        /// Deletes a token for a user.
//        /// </summary>
//        /// <param name="user">The user.</param>
//        /// <param name="loginProvider">The authentication provider for the token.</param>
//        /// <param name="name">The name of the token.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
//        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
//        Task DeleteTokenAsync(string id, CancellationToken cancellationToken);

//        /// <summary>
//        /// Returns the token value.
//        /// </summary>
//        /// <param name="user">The user.</param>
//        /// <param name="loginProvider">The authentication provider for the token.</param>
//        /// <param name="name">The name of the token.</param>
//        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
//        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
//        Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken);
//    }
//}
