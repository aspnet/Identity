using System;
using Identity.ClientApplications.WebSite.Identity.Data;
using Identity.ClientApplications.WebSite.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.AspNetCore.Identity.ClientApplications.FunctionalTest
{
    public class EntityFrameworkSeedReferenceData : IStartupFilter
    {
        public EntityFrameworkSeedReferenceData(
            IdentityApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            ReferenceData seedData)
        {
            SeedContext(dbContext, userManager, seedData);
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return next;
        }

        private void SeedContext(IdentityApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            ReferenceData seedData)
        {
            foreach (var userAndPassword in seedData.UsersAndPasswords)
            {
                userManager
                    .CreateAsync(userAndPassword.user, userAndPassword.password)
                    .GetAwaiter()
                    .GetResult();
            }

            dbContext.Applications.AddRange(seedData.ClientApplications);
            dbContext.SaveChanges();
        }
    }
}
