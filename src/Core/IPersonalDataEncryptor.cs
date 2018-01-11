// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Provides an abstraction used for personal data encryption.
    /// </summary>
    public interface IPersonalDataEncryptor
    {
        /// <summary>
        /// Encrypt the data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Encrypt(string data);

        /// <summary>
        /// Decrypt the data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Decrypt(string data);
    }
}