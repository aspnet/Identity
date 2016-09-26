// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Used to pass information during the SecurityStamp validation event.
    /// </summary>
    public class SecurityStampReplacingPrincipalContext
    {
        /// <summary>
        /// The principal contained in the current cookie.
        /// </summary>
        public ClaimsPrincipal Current { get; set; }

        /// <summary>
        /// The new principal which should replace the current.
        /// </summary>
        public ClaimsPrincipal Replacement { get; set; }
    }
}