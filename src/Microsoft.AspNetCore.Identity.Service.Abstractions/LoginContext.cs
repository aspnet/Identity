// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class LoginContext
    {
        public LoginContext(ClaimsPrincipal user, ClaimsPrincipal applications)
        {
            User = user;
            Applications = applications;
        }

        public ClaimsPrincipal User { get; }
        public ClaimsPrincipal Applications { get; }
    }
}
