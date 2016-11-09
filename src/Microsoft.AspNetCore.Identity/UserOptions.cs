// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Options for user validation.
    /// </summary>
    public class UserOptions
    {
        /// <summary>
        /// Gets or sets the list of allowed characters in the username used to validate user names.
        /// </summary>
        /// <value>
        /// The list of allowed characters in the username used to validate user names.
        /// </value>
        public string AllowedUserNameCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

        /// <summary>
        /// Gets or sets a flag indicating whether the application requires unique emails for its users.
        /// </summary>
        /// <value>
        /// True if the application requires each user to have their own, unique email, otherwise false.
        /// </value>
        public bool RequireUniqueEmail { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the users are allowed to sign in simultaneously
        /// with different devices/browsers.
        /// </summary>
        /// <value>
        /// True if the application requires user sign out before signing in again, otherwise false.
        /// </value>
        public bool RestrictMultipleLogin { get; set; } = false;


        /// <summary>
        /// Gets or sets time before idle user is considered signed out.
        /// </summary>
        /// <value>
        /// Time before idle user is considered signed out.
        /// </value>
        public TimeSpan MultipleLoginTimeout { get; set; } = TimeSpan.FromMinutes(30);
    }
}