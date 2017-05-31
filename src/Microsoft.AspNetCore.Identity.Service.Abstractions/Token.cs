// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.AspNetCore.Identity.Service
{
    public abstract class Token : IEnumerable<Claim>
    {
        private readonly IList<Claim> _claims = new List<Claim>();

        protected Token(IEnumerable<Claim> claims)
        {
            EnsureUniqueClaim(TokenClaimTypes.TokenUniqueId, claims);
            EnsureUniqueClaim(TokenClaimTypes.IssuedAt, claims);
            EnsureUniqueClaim(TokenClaimTypes.Expires, claims);
            EnsureUniqueClaim(TokenClaimTypes.NotBefore, claims);
            _claims = new List<Claim>(claims);
        }

        internal static void EnsureUniqueClaim(string claimType, IEnumerable<Claim> claims, bool required = true)
        {
            var count = 0;
            foreach (var claim in claims)
            {
                if (string.Equals(claimType, claim.Type, StringComparison.Ordinal))
                {
                    count++;
                }

                if (count > 1)
                {
                    break;
                }
            }

            if (count == 0 && required)
            {
                throw new InvalidOperationException($"'{claimType}' is required.");
            }

            if (count > 1)
            {
                throw new InvalidOperationException($"'{claimType}' must be unique.");
            }
        }

        internal static void EnsureRequiredClaim(string claimType, IEnumerable<Claim> claims)
        {
            foreach (var claim in claims)
            {
                if (string.Equals(claimType, claim.Type, StringComparison.Ordinal))
                {
                    return;
                }
            }

            throw new InvalidOperationException($"'{claimType}' not found.");
        }

        public abstract string Kind { get; }
        public virtual string Id => GetClaimValue(TokenClaimTypes.TokenUniqueId);
        public virtual DateTimeOffset IssuedAt => GetClaimValueOrNull(TokenClaimTypes.IssuedAt, v => EpochTime.DateTime(long.Parse(v)));
        public virtual DateTimeOffset Expires => GetClaimValueOrNull(TokenClaimTypes.Expires, v => EpochTime.DateTime(long.Parse(v)));
        public virtual DateTimeOffset NotBefore => GetClaimValueOrNull(TokenClaimTypes.NotBefore, v => EpochTime.DateTime(long.Parse(v)));

        public bool IsOfKind(string tokenType) => Kind.Equals(tokenType, StringComparison.Ordinal);

        public virtual string GetClaimValue(string claimType)
        {
            foreach (var claim in _claims)
            {
                if (string.Equals(claimType, claim.Type, StringComparison.Ordinal))
                {
                    return claim.Value;
                }
            }

            return null;
        }

        protected virtual T GetClaimValueOrNull<T>(string claimType, Func<string, T> valueFactory)
        {
            foreach (var claim in _claims)
            {
                if (string.Equals(claimType, claim.Type, StringComparison.Ordinal))
                {
                    return valueFactory(claim.Value);
                }
            }

            return default(T);
        }

        protected virtual IEnumerable<string> GetClaimValuesOrEmpty(string claimType)
        {
            foreach (var claim in _claims)
            {
                if (string.Equals(claimType, claim.Type, StringComparison.Ordinal))
                {
                    yield return claim.Value;
                }
            }
        }

        public IEnumerator<Claim> GetEnumerator() => _claims.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _claims.GetEnumerator();
    }
}
