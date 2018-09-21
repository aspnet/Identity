// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Identity.UI
{
    /// <summary>
    /// Used by the register page to create a new instance of a user.
    /// </summary>
    /// <typeparam name="TUser">The type of the user to create.</typeparam>
    public abstract class IdentityDefaultUIUserFactory<TUser> where TUser : class
    {
        /// <summary>
        /// Creates a new instance of a user.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="email">The user's email.</param>
        /// <returns></returns>
        public abstract TUser Create(string userName, string email);
    }
}
