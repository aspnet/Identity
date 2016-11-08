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
        /// True if the application requires user sign out before signing in again from another place, otherwise false.
        /// </value>
        public bool DisableMultipleLogin { get; set; } = false;


        /// <summary>
        /// Gets or sets time before user is considered idle
        /// </summary>
        /// <value>
        /// Time before user is considered idle
        /// </value>
        public TimeSpan ActivityTimeout { get; set; } = TimeSpan.FromMinutes(30);


        /// <summary>
        /// Gets or sets limit for maximum concurrent signed in users
        /// </summary>
        /// <value>
        /// Maximum limit of signed in active users (0 for unlimited)
        /// </value>
        public int MaximumSignedIn { get; set; } = 0;

    }
}