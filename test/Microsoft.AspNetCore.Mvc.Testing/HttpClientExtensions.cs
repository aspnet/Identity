using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> SendAsync(this HttpClient client, IHtmlFormElement form)
        {
            var request = form.GetSubmission();
            var httpRequest = new HttpRequestMessage(
                new HttpMethod(request.Method.ToString()),
                request.Target);

            foreach (var header in request.Headers)
            {
                httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            httpRequest.Content = new StreamContent(request.Body);
            httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(request.MimeType);

            return client.SendAsync(httpRequest);
        }
    }
}
