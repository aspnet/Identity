using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using AngleSharp;
using AngleSharp.Extensions;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Xunit;
using AngleSharp.Network;
using System.Threading;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public static class ResponseAssert
    {
        public static Uri IsRedirect(HttpResponseMessage responseMessage)
        {
            Assert.Equal(HttpStatusCode.Redirect, responseMessage.StatusCode);
            return responseMessage.Headers.Location;
        }

        public static void IsOK(HttpResponseMessage responseMessage)
        {
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        public static TCaptured LocationHasQueryParameters<TCaptured>(HttpResponseMessage response, params ValueSpecification[] values)
            where TCaptured : new()
        {
            var queryParameters = LocationHasQueryParameters(response, values);

            var result = new TCaptured();

            return SimpleBind(result, queryParameters.ToDictionary(kvp => kvp.Key, kvp => (string[])kvp.Value, StringComparer.OrdinalIgnoreCase));
        }

        private static TCaptured SimpleBind<TCaptured>(TCaptured result, IDictionary<string, string[]> values)
        {
            foreach (var property in result.GetType().GetProperties())
            {
                if (values.TryGetValue(property.Name, out var value))
                {
                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(result, value.FirstOrDefault());
                    }
                    else if (property.PropertyType == typeof(string[]))
                    {
                        property.SetValue(result, value);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            return result;
        }

        public static IDictionary<string, StringValues> LocationHasQueryParameters(HttpResponseMessage response, params ValueSpecification[] values)
        {
            var location = IsRedirect(response);

            var queryParameters = QueryHelpers.ParseNullableQuery(location.Query);
            foreach (var parameter in values)
            {
                if (!queryParameters.TryGetValue(parameter.Name, out var parameterValues))
                {
                    Assert.True(false, $"Missing parameter '{parameter}'");
                }

                if (!parameter.MatchValue)
                {
                    continue;
                }

                foreach (var expectedValue in parameter.Values)
                {
                    Assert.Contains(expectedValue, parameterValues, parameter.ValueComparer);
                }
            }

            return queryParameters;
        }

        public class ValueSpecification
        {
            public string Name { get; set; }
            public bool MatchValue { get; set; } = true;
            public string[] Values { get; set; }
            public IEqualityComparer<string> ValueComparer { get; set; }

            public static implicit operator ValueSpecification(string name)
            {
                return new ValueSpecification
                {
                    Name = name,
                    MatchValue = false
                };
            }

            public static implicit operator ValueSpecification((string name, string value) tuple) => new ValueSpecification()
            {
                Name = tuple.name,
                Values = new string[] { tuple.value },
                ValueComparer = StringComparer.Ordinal
            };

            public static implicit operator ValueSpecification((string name, string[] values) tuple) => new ValueSpecification()
            {
                Name = tuple.name,
                Values = tuple.values,
                ValueComparer = StringComparer.Ordinal
            };
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
