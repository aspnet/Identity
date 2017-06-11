using System;
using System.Collections.Generic;
using System.Text;
using Identity.ClientApplications.WebSite;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Microsoft.AspNetCore.Identity.ClientApplications.FunctionalTest
{
    public class Test
    {
        [Fact]
        public void CanCreateTestServer()
        {
            // Arrange
            var server = new MvcWebApplicationBuilder<Startup>()
                .UseSolutionRelativeContentRoot(@".\test\WebSites\Identity.ClientApplications.WebSite");

            // Act
            var host = server.Build();

            // Assert
        }
    }
}
