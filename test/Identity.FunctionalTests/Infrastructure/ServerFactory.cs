// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.TestHost;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Infrastructure
{
    class ServerFactory
    {
        internal static HttpClient CreateDefaultClient()
        {
            var builder = WebHostBuilderFactory
                .CreateFromTypesAssemblyEntryPoint<Startup>(new string[] { })
                .UseSolutionRelativeContentRoot(Path.Combine("test", "WebSites", "Identity.DefaultUI.WebSite"));

            var server = new TestServer(builder);
            var client = new HttpClient(new CookieContainerHandler(server.CreateHandler()))
            {
                BaseAddress = new Uri("https://localhost")
            };

            return client;
        }

        internal static TestServer CreateDefaultServer()
        {
            var builder = WebHostBuilderFactory
                .CreateFromTypesAssemblyEntryPoint<Startup>(new string[] { })
                .UseSolutionRelativeContentRoot(Path.Combine("test", "WebSites", "Identity.DefaultUI.WebSite"));

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
