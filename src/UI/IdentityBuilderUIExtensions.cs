// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Default UI extensions to <see cref="IdentityBuilder"/>.
    /// </summary>
    public static class IdentityBuilderUIExtensions
    {
        /// <summary>
        /// Adds a default, self-contained UI for Identity to the application using
        /// Razor Pages in an area named Identity.
        /// </summary>
        /// <remarks>
        /// In order to use the default UI, the application must be using <see cref="Microsoft.AspNetCore.Mvc"/>,
        /// <see cref="Microsoft.AspNetCore.StaticFiles"/> and contain a <c>_LoginPartial</c> partial view that
        /// can be found by the application.
        /// </remarks>
        /// <param name="builder">The <see cref="IdentityBuilder"/>.</param>
        /// <returns>The <see cref="IdentityBuilder"/>.</returns>
        public static IdentityBuilder AddDefaultUI(this IdentityBuilder builder)
        {
            builder.AddSignInManager();
            AddAdditionalApplicationParts(builder);

            builder.Services.ConfigureOptions(
                typeof(IdentityDefaultUIConfigureOptions<>)
                    .MakeGenericType(builder.UserType));
            builder.Services.TryAddTransient<IEmailSender, EmailSender>();

            return builder;
        }

        private static void AddAdditionalApplicationParts(IdentityBuilder builder)
        {
            var thisAssembly = typeof(IdentityBuilderUIExtensions).Assembly;
            var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(thisAssembly, throwOnError: true);

            var mvcBuilder = builder.Services
                .AddMvc()
                .ConfigureApplicationPartManager(partManager =>
                {
                    foreach (var relatedAssembly in relatedAssemblies)
                    {
                        partManager.ApplicationParts.Add(new CompiledRazorAssemblyPart(relatedAssembly));
                    }
                });
        }
    }
}
