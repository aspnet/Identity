// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Provides an abstraction used for personal data encryption.
    /// </summary>
    public interface IPersonalDataProtector
    {
        /// <summary>
        /// Protect the data.
        /// </summary>
        /// <param name="data">The data to protect.</param>
        /// <returns>The protected data.</returns>
        string Protect(string data);

        /// <summary>
        /// Unprotect the data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>The unprotected data.</returns>
        string Unprotect(string data);
    }

    /// <summary>
    /// 
    /// </summary>
    public class DefaultPersonalDataProtector : IPersonalDataProtector
    {
        private readonly IPersonalDataEncryptorKeyRing _keyRing;
        private readonly IPersonalDataEncryptor _encryptor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keyRing"></param>
        /// <param name="encryptor"></param>
        public DefaultPersonalDataProtector(IPersonalDataEncryptorKeyRing keyRing, IPersonalDataEncryptor encryptor)
        {
            _keyRing = keyRing;
            _encryptor = encryptor;
        }

        /// <summary>
        /// Unprotect the data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>The unprotected data.</returns>
        public string Unprotect(string data)
        {
            var split = data.IndexOf(':');
            if (split == -1 || split == data.Length-1)
            {
                throw new InvalidOperationException("Malformed data.");
            }

            var keyId = data.Substring(0, split);
            return _encryptor.Decrypt(keyId, data.Substring(split + 1));
        }

        /// <summary>
        /// Protect the data.
        /// </summary>
        /// <param name="data">The data to protect.</param>
        /// <returns>The protected data.</returns>
        public string Protect(string data)
        {
            var current = _keyRing.CurrentKeyId;
            return current + ":" + _encryptor.Encrypt(current, data);
        }
    }

    /// <summary>
    /// Abstraction used to manage named encryption key rings.
    /// </summary>
    public interface IPersonalDataEncryptorKeyRing
    {
        /// <summary>
        /// Get the current key id.
        /// </summary>
        string CurrentKeyId { get; }

        /// <summary>
        /// Return a specific key.
        /// </summary>
        /// <param name="keyId">The id of the key to fetch.</param>
        /// <returns>The key ring.</returns>
        string this[string keyId] { get; }
    }

    /// <summary>
    /// Used to encrypt/decrypt data with a specific key
    /// </summary>
    public interface IPersonalDataEncryptor
    {
        /// <summary>
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        string Encrypt(string keyId, string data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        string Decrypt(string keyId, string data);
    }
}