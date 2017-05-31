// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity.Service
{
    public interface IApplicationValidator<TApplication>
        where TApplication : class
    {
        Task<IdentityResult> ValidateAsync(ApplicationManager<TApplication> manager, TApplication application);
        Task<IdentityResult> ValidateScopeAsync(ApplicationManager<TApplication> manager, TApplication application, string scope);
        Task<IdentityResult> ValidateRedirectUriAsync(ApplicationManager<TApplication> manager, TApplication application, string redirectUri);
        Task<IdentityResult> ValidateLogoutUriAsync(ApplicationManager<TApplication> manager, TApplication application, string logoutUri);
        Task<IdentityResult> ValidateClaimAsync(ApplicationManager<TApplication> manager, TApplication application, Claim claim);
    }
}
