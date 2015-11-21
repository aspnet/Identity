// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    /// <summary>
    /// Represents a claim that a user possesses. 
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for this user that possesses this claim.</typeparam>
    public class IdentityUserClaim<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the identifier for this user claim.
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets the of the primary key of the user associated with this claim.
        /// </summary>
        public virtual TKey UserId { get; set; }

        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public virtual string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public virtual string ClaimValue { get; set; }

        protected virtual System.Security.Claims.Claim ToClaim()
        {
            return new System.Security.Claims.Claim(this.ClaimType, this.ClaimValue);
        }

        public virtual void FromClaim(System.Security.Claims.Claim other)
        {
            this.ClaimType = other.Type;
            this.ClaimValue = other.Value;
        }

        public static implicit operator System.Security.Claims.Claim( IdentityUserClaim<TKey> item)
        {
            return item.ToClaim();
        }
    }
}