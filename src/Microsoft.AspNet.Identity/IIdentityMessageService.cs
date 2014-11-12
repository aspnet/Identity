// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    public interface IIdentityMessage
    {
        /// <summary>
        ///     Subject
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        ///     Message contents
        /// </summary>
        string Body { get; set; }
    }

    public interface IUserMessageProvider<TUser> where TUser : class
    {
        string Name { get; }

        Task SendAsync(UserManager<TUser> manager, TUser user, IdentityMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface NewUserManagerMethods
    {
        Task<IdentityResult> SendUserMessageAsync(string messageProvider, IdentityMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class EmailMessageProvider<TUser> : IUserMessageProvider<TUser> where TUser : class
    {
        public string Name
        {
            get
            {
                return "Email";
            }
        }

        public Task SendAsync(UserManager<TUser> manager, TUser user, IdentityMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Plug in twillio or whatever
            throw new NotImplementedException();
        }
    }

    public class SmsMessageProvider<TUser> : IUserMessageProvider<TUser> where TUser : class
    {
        public string Name
        {
            get
            {
                return "SMS";
            }
        }

        public Task SendAsync(UserManager<TUser> manager, TUser user, IdentityMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Plug in twillio or whatever
            throw new NotImplementedException();
        }
    }
}