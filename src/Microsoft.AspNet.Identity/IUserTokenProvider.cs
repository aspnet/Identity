// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Interface to generate user tokens
    /// </summary>
    public interface IUserTokenProvider<TUser> where TUser : class
    {
        /// <summary>
        ///     Generate a token for a user
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user, 
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     ValidateAsync and unprotect a token, returns null if invalid
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user, 
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Notifies the user that a token has been generated, i.e. via email or sms, or can no-op
        /// </summary>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task NotifyAsync(string token, UserManager<TUser> manager, TUser user, 
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Returns true if provider can be used for this user, i.e. could require a user to have an email
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IsValidProviderForUserAsync(UserManager<TUser> manager, TUser user, 
            CancellationToken cancellationToken = default(CancellationToken));
    }
}