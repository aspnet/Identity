// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Provides methods to create a claims principal for a given user.
    /// </summary>
    /// <typeparam name="TUser">The type used to represent a user.</typeparam>
    /// <typeparam name="TRole">The type used to represent a role.</typeparam>
    public class UserClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactoryBase<TUser>
        where TUser : class
        where TRole : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserClaimsPrincipalFactory{TUser, TRole}"/> class.
        /// </summary>
        /// <param name="userManager">The <see cref="UserManager{TUser}"/> to retrieve user information from.</param>
        /// <param name="roleManager">The <see cref="RoleManager{TRole}"/> to retrieve a user's roles from.</param>
        /// <param name="optionsAccessor">The configured <see cref="IdentityOptions"/>.</param>
        public UserClaimsPrincipalFactory(
            UserManager<TUser> userManager, 
            RoleManager<TRole> roleManager, 
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
            if (roleManager == null)
            {
                throw new ArgumentNullException(nameof(roleManager));
            }

            RoleManager = roleManager;
        }

        /// <summary>
        /// Gets the <see cref="RoleManager{TRole}"/> for this factory.
        /// </summary>
        /// <value>
        /// The current <see cref="RoleManager{TRole}"/> for this factory instance.
        /// </value>
        public RoleManager<TRole> RoleManager { get; private set; }

        /// <summary>
        /// Adds the roles into given <see cref="ClaimsIdentity"/> instance and returns a new one.
        /// </summary>
        /// <param name="user">The user to create a <see cref="ClaimsPrincipal"/> from.</param>
        /// <param name="baseIdentity">The base <see cref="ClaimsIdentity"/> to add roles into.</param>
        protected override async Task<ClaimsIdentity> AddRolesAsync(TUser user, ClaimsIdentity baseIdentity)
        {
            if (UserManager.SupportsUserRole)
            {
                var roles = await UserManager.GetRolesAsync(user);
                foreach (var roleName in roles)
                {
                    baseIdentity.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, roleName));
                    if (RoleManager.SupportsRoleClaims)
                    {
                        var role = await RoleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            baseIdentity.AddClaims(await RoleManager.GetClaimsAsync(role));
                        }
                    }
                }
            }

            return baseIdentity;
        }
    }
}