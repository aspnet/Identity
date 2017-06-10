// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Identity.Service.IntegratedWebClient.Internal
{
    public class IntegratedWebClientModelConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                foreach (var action in controller.Actions)
                {
                    Apply(action);
                }
            }
        }

        private void Apply(ActionModel action)
        {
            var parameters = action.Parameters.Where(p => p.Attributes.OfType<EnableIntegratedWebClientAttribute>().Any());
            if (parameters.Any())
            {
                action.Filters.Add(new IntegratedWebClientRedirectFilter(
                    parameters.Select(p => p.ParameterName)));
            }
        }
    }
}
