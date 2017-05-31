// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.AzureKeyVault;
using Microsoft.AspNetCore.Identity.Service.AzureKeyVault.Internal;
using Microsoft.AspNetCore.Identity.Service.Signing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityClientApplicationsBuilderExtensions
    {
        public static IIdentityClientApplicationsBuilder AddKeyVault(this IIdentityClientApplicationsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var services = builder.Services;
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ConfigureDefaultOptions<KeyVaultSigningCredentialsSourceOptions>, DefaultSetup>());
            services.TryAddSingleton<ISigningCredentialsSource, KeyVaultSigningCredentialSource>();
            return builder;
        }

        public static IIdentityClientApplicationsBuilder AddKeyVault(this IIdentityClientApplicationsBuilder builder, Action<KeyVaultSigningCredentialsSourceOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddKeyVault();
            builder.Services.Configure(configure);

            return builder;
        }

        private class DefaultSetup : ConfigureDefaultOptions<KeyVaultSigningCredentialsSourceOptions>
        {
            private const string SectionKey = "Microsoft:AspNetCore:Identity:Service:SigningKeys:KeyVault";

            public DefaultSetup(IConfiguration configuration) :
                base(options => configuration.GetSection(SectionKey).Bind(options))
            { }
        }
    }
}
