// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test;
using Microsoft.AspNetCore.Identity.Service.Specification.Tests;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Testing;
using Microsoft.AspNetCore.Testing.xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Identity.Service.EntityFrameworkCore.Test
{
    public class ApplicationStoreTest : IdentityServiceSpecificationTestBase<IdentityUser, IdentityClientApplication>, IClassFixture<ScratchDatabaseFixture>
    {
        private readonly ScratchDatabaseFixture _fixture;
        public static readonly ApplicationErrorDescriber ErrorDescriber = new ApplicationErrorDescriber();

        public ApplicationStoreTest(ScratchDatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        protected override void AddApplicationStore(IServiceCollection services, object context = null)
        {
            services.AddSingleton<IApplicationStore<IdentityClientApplication>>(
                new ApplicationStore<IdentityClientApplication, IdentityClientApplicationScope<string>, IdentityClientApplicationClaim<string>, IdentityClientApplicationRedirectUri<string>, IdentityClientApplicationsDbContext<IdentityUser, IdentityClientApplication>, string>((IdentityClientApplicationsDbContext<IdentityUser, IdentityClientApplication>)context, new ApplicationErrorDescriber()));
        }

        public IdentityClientApplicationsDbContext<IdentityUser, IdentityClientApplication> CreateContext(bool delete = false)
        {
            var db = DbUtil.Create<TestContext>(_fixture.ConnectionString);
            if (delete)
            {
                db.Database.EnsureDeleted();
            }
            db.Database.EnsureCreated();
            return db;
        }

        protected override IdentityClientApplication CreateTestApplication()
        {
            return new IdentityClientApplication
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };
        }

        protected override object CreateTestContext()
        {
            return CreateContext();
        }

        private class TestContext : IdentityClientApplicationsDbContext<IdentityUser, IdentityClientApplication>
        {
            public TestContext(DbContextOptions options) : base(options) { }
        }

        protected override bool ShouldSkipDbTests() => TestPlatformHelper.IsMono || !TestPlatformHelper.IsWindows;

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public async Task ConcurrentUpdatesWillFail()
        {
            var application = CreateTestApplication();
            using (var db = CreateContext())
            {
                var manager = CreateManager(db);
                IdentityResultAssert.IsSuccess(await manager.CreateAsync(application));
            }
            using (var db = CreateContext())
            using (var db2 = CreateContext())
            {
                var manager1 = CreateManager(db);
                var manager2 = CreateManager(db2);
                var application1 = await manager1.FindByIdAsync(application.Id);
                var application2 = await manager2.FindByIdAsync(application.Id);
                Assert.NotNull(application1);
                Assert.NotNull(application2);
                Assert.NotSame(application1, application2);
                application1.Name = Guid.NewGuid().ToString();
                application2.Name = Guid.NewGuid().ToString();
                IdentityResultAssert.IsSuccess(await manager1.UpdateAsync(application1));
                IdentityResultAssert.IsFailure(await manager2.UpdateAsync(application2), ErrorDescriber.ConcurrencyFailure());
            }
        }

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public async Task ConcurrentUpdatesWillFailWithDetachedApplication()
        {
            var application = CreateTestApplication();
            using (var db = CreateContext())
            {
                var manager = CreateManager(db);
                IdentityResultAssert.IsSuccess(await manager.CreateAsync(application));
            }
            using (var db1 = CreateContext())
            using (var db2 = CreateContext())
            {
                var manager1 = CreateManager(db1);
                var manager2 = CreateManager(db2);
                var application2 = await manager2.FindByIdAsync(application.Id);
                Assert.NotNull(application2);
                Assert.NotSame(application, application2);
                application.Name= Guid.NewGuid().ToString();
                application2.Name = Guid.NewGuid().ToString();
                IdentityResultAssert.IsSuccess(await manager1.UpdateAsync(application));
                IdentityResultAssert.IsFailure(await manager2.UpdateAsync(application2), ErrorDescriber.ConcurrencyFailure());
            }
        }

        [ConditionalFact]
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        [OSSkipCondition(OperatingSystems.Linux)]
        [OSSkipCondition(OperatingSystems.MacOSX)]
        public async Task DeleteAModifiedApplicationWillFail()
        {
            var application = CreateTestApplication();
            using (var db = CreateContext())
            {
                var manager = CreateManager(db);
                IdentityResultAssert.IsSuccess(await manager.CreateAsync(application));
            }
            using (var db = CreateContext())
            using (var db2 = CreateContext())
            {
                var manager1 = CreateManager(db);
                var manager2 = CreateManager(db2);
                var application1 = await manager1.FindByIdAsync(application.Id);
                var application2 = await manager2.FindByIdAsync(application.Id);
                Assert.NotNull(application1);
                Assert.NotNull(application2);
                Assert.NotSame(application1, application2);
                application1.Name = Guid.NewGuid().ToString();
                IdentityResultAssert.IsSuccess(await manager1.UpdateAsync(application1));
                IdentityResultAssert.IsFailure(await manager2.DeleteAsync(application2), ErrorDescriber.ConcurrencyFailure());
            }
        }
    }
}
