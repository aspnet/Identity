// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Provides an abstraction for hashing passwords.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Returns a hashed representation of the supplied <paramref name="password"/>
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>A hashed representation of the supplied <paramref name="password"/></returns>
        string HashPassword(string password);

        /// <summary>
        /// Returns a <see cref="PasswordVerificationResult"/> indicating the result of a password hash comparison.
        /// </summary>
        /// <param name="hashedPassword">The hash value for a user's stored password.</param>
        /// <param name="providedPassword">The password supplied for comparison.</param>
        /// <returns>A <see cref="PasswordVerificationResult"/> indicating the result of a password hash comparison.</returns>
        /// <remarks>Implementations of this method should be time consistent.</remarks>
        PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}