// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Used for authenticator secret generation and code verification.
    /// </summary>
    public interface IAuthenticatorVerification // REVIEW: name!
    {
        /// <summary>
        /// Generates a new security secret for the authenticator.
        /// </summary>
        /// <returns>The new secret.</returns>
        string GenerateSecret();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool VerifyCode(string secret, int code);
    }
}