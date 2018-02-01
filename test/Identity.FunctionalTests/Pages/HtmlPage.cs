using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using AngleSharp.Dom.Html;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Pages
{
    public class HtmlPage
    {
        public HtmlPage(HttpClient client, IHtmlDocument document, GlobalContext context)
        {
            Client = client;
            Document = document;
            Context = context;
        }

        public HttpClient Client { get; }
        public IHtmlDocument Document { get; }
        public GlobalContext Context { get; }
    }
}
