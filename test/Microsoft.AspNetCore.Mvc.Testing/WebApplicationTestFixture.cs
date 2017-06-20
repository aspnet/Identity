// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests
{
    public class WebApplicationTestFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly TestServer _server;

        public WebApplicationTestFixture()
            : this("*.sln")
        {
        }

        public WebApplicationTestFixture(string solutionSearchPattern)
            : this(solutionSearchPattern, Path.Combine("test", "WebSites"))
        {
        }

        protected WebApplicationTestFixture(string solutionSearchPattern, string solutionRelativePath)
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;

            // This step assumes project name = assembly name.
            var projectName = startupAssembly.GetName().Name;
            var projectPath = Path.Combine(solutionRelativePath, projectName);
            var builder = new MvcWebApplicationBuilder<TStartup>()
                .UseSolutionRelativeContentRoot(projectPath);

            _server = builder.Build();
            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}
