using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Identity.DefaultUI.WebSite;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class RegistrationTests
    {
        [Fact]
        public async Task CanRegisterAUser()
        {
            // Arrange
            var builder = WebHostBuilderFactory
                .CreateFromTypesAssemblyEntryPoint<Startup>(new string[] { });

            var server = new TestServer(builder);
            var client = server.CreateClient();

            // Act & Assert
            var index = await client.GetAsync("/");

        }
    }
}
