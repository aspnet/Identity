// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Diagnostics.Identity.Service.Internal;

namespace Microsoft.AspNetCore.Builder
{
    public static class IdentityApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDevelopmentCertificateErrorPage(
            this IApplicationBuilder builder)
        {
            builder.UseMiddleware<DeveloperCertificateMiddleware>();
            return builder;
        }
    }
}
