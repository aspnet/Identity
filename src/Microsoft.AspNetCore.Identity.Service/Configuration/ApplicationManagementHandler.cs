// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.AspNetCore.Identity.Service.Configuration
{
    public class ApplicationManagementHandler : AuthorizationHandler<ApplicationManagementRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ApplicationManagementRequirement requirement)
        {
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
