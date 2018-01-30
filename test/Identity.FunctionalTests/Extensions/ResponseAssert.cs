// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Network;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public static class ResponseAssert
    {
        public static Uri IsRedirect(HttpResponseMessage responseMessage)
        {
            Assert.Equal(HttpStatusCode.Redirect, responseMessage.StatusCode);
            return responseMessage.Headers.Location;
        }

        public static IHtmlDocument IsHtmlDocument(HttpResponseMessage response)
        {
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("text/html", response.Content.Headers.ContentType.MediaType);

            var document = BrowsingContext.New()
                .OpenAsync(MapResponse(), CancellationToken.None).Result;
            return Assert.IsAssignableFrom<IHtmlDocument>(document);

            IResponse MapResponse() =>
                VirtualResponse.Create(r =>
                    r.Address(response.RequestMessage.RequestUri)
                        .Content(response.Content.ReadAsStreamAsync().Result)
                        .Status(response.StatusCode)
                        .Headers(response.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.First()))
                        .Headers(response.Content.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.First())));
        }
    }
}
