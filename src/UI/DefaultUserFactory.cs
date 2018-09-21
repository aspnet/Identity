// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Identity.UI
{
    public class FuncUserFactory<TUser> : IdentityDefaultUIUserFactory<TUser> where TUser : class
    {
        private readonly Func<string, string, TUser> _createFunc;

        public FuncUserFactory(Func<string, string, TUser> createFunc)
            => _createFunc = createFunc;

        public override TUser Create(string userName, string email)
            => _createFunc(userName, email);
    }


    /// <summary>
    /// Options for the default Identity UI
    /// </summary>
    public class DefaultUserFactory<TUser, TKey> : IdentityDefaultUIUserFactory<TUser> where TUser : IdentityUser<TKey>, new() where TKey : IEquatable<TKey>
    {
        public override TUser Create(string userName, string email)
        {
            var user = new TUser();
            user.UserName = userName;
            user.Email = email;
            return user;
        }
    }
}
