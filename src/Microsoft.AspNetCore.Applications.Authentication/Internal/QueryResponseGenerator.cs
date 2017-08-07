// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.AspNetCore.Applications.Authentication.Internal
{
    /// <summary>
    /// Generator for Open ID Connect query response types.
    /// </summary>
    public class QueryResponseGenerator
    {
        /// <summary>
        /// Generates an <see cref="StatusCodes.Status302Found"/> response to <paramref name="redirectUri"/> with the
        /// <paramref name="parameters"/> encoded in the query string of the location URI as URL encoded name value pairs.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/></param>
        /// <param name="redirectUri">The <see cref="Uri"/> for the location header.</param>
        /// <param name="parameters">The values to include as URL encoded name value pairs on the query.</param>
        /// <returns>A <see cref="Task"/> that when completed will have created a <see cref="StatusCodes.Status302Found"/>
        /// <see cref="HttpResponse"/>.
        /// </returns>
        public void GenerateResponse(
            HttpContext context,
            string redirect,
            IEnumerable<KeyValuePair<string,string>> parameters)
        {
            var uri = new Uri(redirect);

            var queryCollection = QueryHelpers.ParseQuery(uri.Query);
            var queryBuilder = new QueryBuilder();
            foreach (var kvp in parameters)
            {
                if (!ShouldSkipKey(kvp.Key))
                {
                    queryBuilder.Add(kvp.Key, kvp.Value);
                }
            }

            var queryString = queryBuilder.ToQueryString().ToUriComponent();
            var redirectUri = $"{uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped)}{queryString}";
            context.Response.Redirect(redirectUri);
        }

        private bool ShouldSkipKey(string key)
        {
            return string.Equals(key, OpenIdConnectParameterNames.RedirectUri, StringComparison.OrdinalIgnoreCase);
        }
    }
}
