// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    public interface IAuthenticationManager
    {
        void SignIn(ClaimsIdentity identity, bool isPersistent);
        void SignOut(string authenticationType);

        // remember browser for two factor
        void ForgetClient();
        void RememberClient(string userId);
        Task<bool> IsClientRememeberedAsync(string userId, 
            CancellationToken cancellationToken = default(CancellationToken));

        // half cookie
        Task StoreTwoFactorInfo(TwoFactorAuthenticationInfo info, CancellationToken cancellationToken = default(CancellationToken));
        Task<TwoFactorAuthenticationInfo> RetrieveTwoFactorInfo(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class TwoFactorAuthenticationInfo
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
    }
}