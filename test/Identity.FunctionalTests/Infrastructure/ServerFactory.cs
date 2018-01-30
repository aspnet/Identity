// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Infrastructure
{
    public class ServerFactory
    {
        public static HttpClient CreateDefaultClient() =>
            CreateDefaultClient(CreateDefaultServer());

        public static TestServer CreateDefaultServer()
        {
            var builder = WebHostBuilderFactory
                .CreateFromTypesAssemblyEntryPoint<Startup>(new string[] { })
                .UseSolutionRelativeContentRoot(Path.Combine("test", "WebSites", "Identity.DefaultUI.WebSite"))
                .ConfigureServices(sc => sc.AddSingleton<EnsureDatabaseStartupFilter>());

            var server = new TestServer(builder);
            return server;
        }

        public static HttpClient CreateDefaultClient(TestServer server)
        {
            var client = new HttpClient(new CookieContainerHandler(server.CreateHandler()))
            {
                BaseAddress = new Uri("https://localhost")
            };

            return client;
        }
    }
}
