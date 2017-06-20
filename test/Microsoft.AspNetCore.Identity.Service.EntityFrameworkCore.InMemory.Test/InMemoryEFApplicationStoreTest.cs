// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Service.Specification.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity.Service.EntityFrameworkCore.InMemory.Test
{
    public class InMemoryEFApplicationStoreTest : IdentityServiceSpecificationTestBase<IdentityUser, IdentityClientApplication, string, string>
    {
        protected override void AddApplicationStore(IServiceCollection services, object context = null)
        {
            services.AddSingleton<IApplicationStore<IdentityClientApplication>>(
                new ApplicationStore<IdentityClientApplication, IdentityClientApplicationScope<string>, IdentityClientApplicationClaim<string>, IdentityClientApplicationRedirectUri<string>, InMemoryContext, string>((InMemoryContext)context, new ApplicationErrorDescriber()));
        }

        protected override IdentityClientApplication CreateTestApplication()
        {
            return new IdentityClientApplication
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
            };
        }

        protected override object CreateTestContext()
        {
            return new InMemoryContext(new DbContextOptionsBuilder().Options);
        }
    }
}
