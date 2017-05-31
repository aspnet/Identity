// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity.Service
{
    public class LoginResult
    {
        private static LoginResult RequireLogin = new LoginResult();

        private LoginResult(AuthorizationRequestError error)
        {
            Error = error;
            Status = LoginStatus.Forbidden;
        }

        private LoginResult(ClaimsPrincipal user, ClaimsPrincipal application)
        {
            User = user;
            Application = application;
            Status = LoginStatus.Authorized;
        }

        private LoginResult()
        {
            Status = LoginStatus.LoginRequired;
        }

        public static LoginResult Forbidden(AuthorizationRequestError error)
        {
            return new LoginResult(error);
        }

        public static LoginResult Authorized(ClaimsPrincipal user, ClaimsPrincipal application)
        {
            return new LoginResult(user, application);
        }

        public static LoginResult LoginRequired() => RequireLogin;

        public LoginStatus Status { get; set; }

        public AuthorizationRequestError Error { get; set; }

        public ClaimsPrincipal User { get; set; }

        public ClaimsPrincipal Application { get; set; }
    }
}
