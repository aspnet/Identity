using System;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Identity.Claims;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Contains extension methods for <see cref="IdentityBuilder"/>
    /// </summary>
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        /// Defines mappings for claims from the <typeparamref name="TUser"/> properties
        /// into <see cref="Claim"/>.
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <param name="builder"></param>
        /// <param name="configure">An action to configure the mappings of properties
        /// from the user into claims.</param>
        /// <returns>The <see cref="IdentityBuilder"/>.</returns>
        public static IdentityBuilder AddClaimsMappings<TUser>(
            this IdentityBuilder builder,
            Action<UserClaimsModel<TUser>> configure) where TUser : class
        {
            builder.Services.AddSingleton<IUserClaimsMappingsProvider<TUser>>(
                new ActionUserClaimsMappingProvider<TUser>(configure));

            return builder;
        }
    }

    internal class Samples
    {
        public void Main()
        {
            //AddIdentity()
            //    .AddClaimsMappings<ApplicationUser>(m => 
            //        m.Map(u => new Claim[] { new Claim("age", u.Age.ToString()) }));

            //AddIdentity()
            //    .AddClaimsMappings<ApplicationUser>(m => 
            //        m.Map(u => new Claim[] { new Claim("age", u.Age.ToString()) }));
        }

        private IdentityBuilder AddIdentity() => null;
    }

    internal class ApplicationUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
    }
}
