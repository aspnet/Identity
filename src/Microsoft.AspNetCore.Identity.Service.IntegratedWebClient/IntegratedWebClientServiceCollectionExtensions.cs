// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity.Service.IntegratedWebClient;
using Microsoft.AspNetCore.Identity.Service.IntegratedWebClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IntegratedWebClientServiceCollectionExtensions
    {
        public static AuthenticationBuilder WithIntegratedWebClient(
            this AuthenticationBuilder builder,
            Action<IntegratedWebClientOptions> action)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            builder.WithIntegratedWebClient();
            builder.Services.Configure(action);

            return builder;
        }

        public static AuthenticationBuilder WithIntegratedWebClient(this AuthenticationBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Scoped<IConfigureOptions<OpenIdConnectOptions>, IntegratedWebClientOpenIdConnectOptionsSetup>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<MvcOptions>, IntegratedWebclientMvcOptionsSetup>());
            return builder;
        }
    }
}
