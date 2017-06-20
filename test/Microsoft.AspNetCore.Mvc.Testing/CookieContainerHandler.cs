using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    public class CookieContainerHandler : DelegatingHandler
    {
        public CookieContainerHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        public CookieContainer Container { get; } = new CookieContainer();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var cookieHeader = Container.GetCookieHeader(request.RequestUri);
            request.Headers.Add("Cookie", cookieHeader);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
            {
                foreach (var header in setCookieHeaders)
                {
                    CookieContainer helper = new CookieContainer();
                    helper.SetCookies(response.RequestMessage.RequestUri, header);
                    CookieCollection cookies = helper.GetCookies(response.RequestMessage.RequestUri);
                    foreach (Cookie cookie in cookies)
                    {
                        Container.Add(response.RequestMessage.RequestUri, cookie);
                    }
                }
            }

            return response;
        }
    }
}