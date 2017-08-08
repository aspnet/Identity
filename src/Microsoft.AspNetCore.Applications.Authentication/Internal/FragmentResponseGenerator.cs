// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.AspNetCore.Applications.Authentication.Internal
{
    /// <summary>
    /// Generator for Open ID Connect fragment response types.
    /// </summary>
    public class FragmentResponseGenerator
    {
        private readonly UrlEncoder _urlEncoder;

        /// <summary>
        /// Creates anew instance of <see cref="FragmentResponseGenerator"/>.
        /// </summary>
        /// <param name="urlEncoder">The <see cref="UrlEncoder"/>used to encode parameter values on the fragment.</param>
        public FragmentResponseGenerator(UrlEncoder urlEncoder)
        {
            _urlEncoder = urlEncoder;
        }

        /// <summary>
        /// Generates an <see cref="StatusCodes.Status302Found"/> response to <paramref name="redirectUri"/> with the
        /// <paramref name="parameters"/> encoded in the fragment of the location URI as URL encoded name value pairs.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/></param>
        /// <param name="redirectUri">The <see cref="Uri"/> for the location header.</param>
        /// <param name="parameters">The values to include as URL encoded name value pairs on the fragment.</param>
        /// <returns>A <see cref="Task"/> that when completed will have created a <see cref="StatusCodes.Status302Found"/>
        /// <see cref="HttpResponse"/>.
        /// </returns>
        public void GenerateResponse(
            HttpContext context,
            string redirectUri,
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var builder = new StringBuilder();
            builder.Append(redirectUri);
            builder.Append('#');

            var enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!ShouldSkipKey(enumerator.Current.Key))
                {
                    builder.Append(_urlEncoder.Encode(enumerator.Current.Key));
                    builder.Append('=');
                    builder.Append(_urlEncoder.Encode(enumerator.Current.Value));
                    break;
                }
            }

            while (enumerator.MoveNext())
            {
                if (!ShouldSkipKey(enumerator.Current.Key))
                {
                    builder.Append('&');
                    builder.Append(_urlEncoder.Encode(enumerator.Current.Key));
                    builder.Append('=');
                    builder.Append(_urlEncoder.Encode(enumerator.Current.Value));
                }
            }

            context.Response.Redirect(builder.ToString());
        }

        private bool ShouldSkipKey(string key)
        {
            return string.Equals(key, OpenIdConnectParameterNames.RedirectUri, StringComparison.OrdinalIgnoreCase);
        }
    }
}
