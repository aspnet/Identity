// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class ApplicationErrorDescriber
    {
        public virtual IdentityError InvalidApplicationName(string applicationName) => new IdentityError
        {
            Code = nameof(InvalidApplicationName),
            Description = $"The application name '{applicationName}' is not valid."
        };

        public virtual IdentityError DuplicateApplicationName(string applicationName) => new IdentityError
        {
            Code = nameof(DuplicateApplicationName),
            Description = $"An application with name '{applicationName}' already exists."
        };

        public virtual IdentityError InvalidApplicationClientId(string clientId) => new IdentityError
        {
            Code = nameof(InvalidApplicationClientId),
            Description = $"The application client ID '{clientId}' is not valid."
        };

        public virtual IdentityError DuplicateApplicationClientId(string clientId) => new IdentityError
        {
            Code = nameof(DuplicateApplicationClientId),
            Description = $"An application with client ID '{clientId}' already exists."
        };

        public virtual IdentityError DuplicateLogoutUri(string logoutUri) => new IdentityError
        {
            Code = nameof(DuplicateLogoutUri),
            Description = $"The application already contains a logout uri '{logoutUri}'."
        };

        public virtual IdentityError InvalidLogoutUri(string logoutUri) => new IdentityError
        {
            Code = nameof(InvalidLogoutUri),
            Description = $"The logout uri '{logoutUri}' is not valid."
        };

        public virtual IdentityError DifferentDomains() => new IdentityError
        {
            Code = nameof(DifferentDomains),
            Description = $"All the URIs in an application must have the same domain."
        };

        public virtual IdentityError DuplicateRedirectUri(string redirectUri) => new IdentityError
        {
            Code = nameof(DuplicateRedirectUri),
            Description = $"The application already contains a redirect uri '{redirectUri}'."
        };

        public virtual IdentityError InvalidRedirectUri(string redirectUri) => new IdentityError
        {
            Code = nameof(InvalidRedirectUri),
            Description = $"The redirect URI '{redirectUri}' is not valid."
        };

        public virtual IdentityError InvalidScope(string scope) => new IdentityError
        {
            Code = nameof(InvalidScope),
            Description = $"The scope '{scope}' is not valid."
        };

        public virtual IdentityError DuplicateScope(string scope) => new IdentityError
        {
            Code = nameof(DuplicateScope),
            Description = $"The application already contains a scope '{scope}'."
        };

        public virtual IdentityError ApplicationAlreadyHasClientSecret() => new IdentityError {
            Code = nameof(ApplicationAlreadyHasClientSecret),
            Description = $"The application already has a client secret."
        };

        public virtual IdentityError RedirectUriNotFound(string redirectUri) => new IdentityError
        {
            Code = nameof(RedirectUriNotFound),
            Description = $"The redirect uri '{redirectUri}' can not be found."
        };

        public virtual IdentityError LogoutUriNotFound(string logoutUri) => new IdentityError
        {
            Code = nameof(LogoutUriNotFound),
            Description = $"The logout uri '{logoutUri}' can not be found."
        };

        public virtual IdentityError ConcurrencyFailure() => new IdentityError
        {
            Code = nameof(ConcurrencyFailure),
            Description = $"Optimistic concurrency failure, object has been modified."
        };

        public virtual IdentityError ScopeNotFound(string scope) => new IdentityError
        {
            Code = nameof(ScopeNotFound),
            Description = $"The scope '{scope}' can not be found."
        };
    }
}
