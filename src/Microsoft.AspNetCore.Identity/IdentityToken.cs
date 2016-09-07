// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Represents a token with id, type, and value
    /// </summary>
    public class IdentityToken
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="id">The unique token identifier.</param>
        /// <param name="type">The type of the token.</param>
        /// <param name="value">The value of the token.</param>
        public IdentityToken(string id, string type, string value)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            Id = id;
            Type = type;
            Value = value;
        }

        /// <summary>
        /// The unique identifier for the token.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The token type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The token value
        /// </summary>
        public string Value { get; }
    }
}