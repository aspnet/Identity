// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Specifies options for password requirements.
    /// </summary>
    public class PasswordOptions
    {
        /// <summary>
        /// Gets or sets the minimum length a password must be. Defaults to 6.
        /// </summary>
        public int RequiredLength { get; set; } = 6;

        /// <summary>
        /// Gets or sets the minimum number of unique chars a password must comprised of. Defaults to 1.
        /// </summary>
        public int RequiredUniqueChars { get; set; } = 1;

        /// <summary>
        /// Gets or sets a flag indicating if passwords must contain a non-alphanumeric character. Defaults to true.
        /// </summary>
        /// <value>True if passwords must contain a non-alphanumeric character, otherwise false.</value>
        public bool RequireNonAlphanumeric { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if passwords must contain a lower case ASCII character. Defaults to true.
        /// </summary>
        /// <value>True if passwords must contain a lower case ASCII character.</value>
        public bool RequireLowercase { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if passwords must contain a upper case ASCII character. Defaults to true.
        /// </summary>
        /// <value>True if passwords must contain a upper case ASCII character.</value>
        public bool RequireUppercase { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if passwords must contain a digit. Defaults to true.
        /// </summary>
        /// <value>True if passwords must contain a digit.</value>
        public bool RequireDigit { get; set; } = true;
    }
}