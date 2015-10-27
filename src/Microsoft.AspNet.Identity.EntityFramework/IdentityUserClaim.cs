// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    /// <summary>
    ///     EntityType that represents one specific user claim
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class IdentityUserClaim<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Primary key
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        ///     User Id for the user who owns this claim
        /// </summary>
        public virtual TKey UserId { get; set; }

        /// <summary>
        ///     Claim type
        /// </summary>
        public virtual string ClaimType { get; set; }

        /// <summary>
        ///     Claim value
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