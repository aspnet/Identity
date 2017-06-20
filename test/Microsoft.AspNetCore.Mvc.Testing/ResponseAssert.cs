using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    public static class ResponseAssert
    {
        public static IHtmlDocument IsHtmlDocument(HttpResponseMessage response)
        {
            try
            {
                var parser = new HtmlParser();
                return parser.Parse(response.Content.ReadAsStreamAsync().GetAwaiter().GetResult());
            }
            catch
            {
                Assert.True(false, "Invalid Html document.");
            }

            return null;
        }
    }
}
