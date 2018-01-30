using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Infrastructure
{
    public static class FunctionalTestsServiceCollectionExtensions
    {
        public static IServiceCollection SetupTestDatabase(this IServiceCollection services)
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseInMemoryDatabase("test", memoryOptions => { }));

            return services;
        }
    }
}
