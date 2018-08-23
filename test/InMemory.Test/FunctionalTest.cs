// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.AspNetCore.Identity.InMemory
{
    public class FunctionalTest
    {
        const string TestPassword = "1qaz!QAZ";

        [Fact]
        public async Task CanChangePasswordOptions()
        {
            var clock = new TestClock();
            var server = CreateServer(services => services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            }));

            var transaction1 = await SendAsync(server, "http://example.com/createSimple");

            // Assert
            Assert.Equal(HttpStatusCode.OK, transaction1.Response.StatusCode);
            Assert.Null(transaction1.SetCookie);
        }

        [Fact]
        public async Task CookieContainsRoleClaim()
        {
            var clock = new TestClock();
            var server = CreateServer(null, null, null, testCore: true);

            var transaction1 = await SendAsync(server, "http://example.com/createMe");
            Assert.Equal(HttpStatusCode.OK, transaction1.Response.StatusCode);
            Assert.Null(transaction1.SetCookie);

            var transaction2 = await SendAsync(server, "http://example.com/pwdLogin/false");
            Assert.Equal(HttpStatusCode.OK, transaction2.Response.StatusCode);
            Assert.NotNull(transaction2.SetCookie);
            Assert.DoesNotContain("; expires=", transaction2.SetCookie);

            var transaction3 = await SendAsync(server, "http://example.com/me", transaction2.CookieNameValue);
            Assert.Equal("hao", FindClaimValue(transaction3, ClaimTypes.Name));
            Assert.Equal("role", FindClaimValue(transaction3, ClaimTypes.Role));
            Assert.Null(transaction3.SetCookie);
        }

        [Fact]
        public async Task CanCreateMeLoginAndCookieStopsWorkingAfterExpiration()
        {
            var clock = new TestClock();
            var server = CreateServer(services =>
            {
                services.ConfigureApplicationCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                    options.SlidingExpiration = false;
                });
                services.AddSingleton<ISystemClock>(clock);
            });

            var transaction1 = await SendAsync(server, "http://example.com/createMe");
            Assert.Equal(HttpStatusCode.OK, transaction1.Response.StatusCode);
            Assert.Null(transaction1.SetCookie);

            var transaction2 = await SendAsync(server, "http://example.com/pwdLogin/false");
            Assert.Equal(HttpStatusCode.OK, transaction2.Response.StatusCode);
            Assert.NotNull(transaction2.SetCookie);
            Assert.DoesNotContain("; expires=", transaction2.SetCookie);

            var transaction3 = await SendAsync(server, "http://example.com/me", transaction2.CookieNameValue);
            Assert.Equal("hao", FindClaimValue(transaction3, ClaimTypes.Name));
            Assert.Null(transaction3.SetCookie);

            clock.Add(TimeSpan.FromMinutes(7));

            var transaction4 = await SendAsync(server, "http://example.com/me", transaction2.CookieNameValue);
            Assert.Equal("hao", FindClaimValue(transaction4, ClaimTypes.Name));
            Assert.Null(transaction4.SetCookie);

            clock.Add(TimeSpan.FromMinutes(7));

            var transaction5 = await SendAsync(server, "http://example.com/me", transaction2.CookieNameValue);
            Assert.Null(FindClaimValue(transaction5, ClaimTypes.Name));
            Assert.Null(transaction5.SetCookie);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CanCreateMeLoginAndSecurityStampExtendsExpiration(bool rememberMe)
        {
            var clock = new TestClock();
            var server = CreateServer(services => services.AddSingleton<ISystemClock>(clock));

            var transaction1 = await SendAsync(server, "http://example.com/createMe");
            Assert.Equal(HttpStatusCode.OK, transaction1.Response.StatusCode);
            Assert.Null(transaction1.SetCookie);

            var transaction2 = await SendAsync(server, "http://example.com/pwdLogin/" + rememberMe);
            Assert.Equal(HttpStatusCode.OK, transaction2.Response.StatusCode);
            Assert.NotNull(transaction2.SetCookie);
            if (rememberMe)
            {
                Assert.Contains("; expires=", transaction2.SetCookie);
            }
            else
            {
                Assert.DoesNotContain("; expires=", transaction2.SetCookie);
            }

            var transaction3 = await SendAsync(server, "http://example.com/me", transaction2.CookieNameValue);
            Assert.Equal("hao", FindClaimValue(transaction3, ClaimTypes.Name));
            Assert.Null(transaction3.SetCookie);

            // Make sure we don't get a new cookie yet
            clock.Add(TimeSpan.FromMinutes(10));
            var transaction4 = await SendAsync(server, "http://example.com/me", transaction2.CookieNameValue);
            Assert.Equal("hao", FindClaimValue(transaction4, ClaimTypes.Name));
            Assert.Null(transaction4.SetCookie);

            // Go past SecurityStampValidation interval and ensure we get a new cookie
            clock.Add(TimeSpan.FromMinutes(21));

            var transaction5 = await SendAsync(server, "http://example.com/me", transaction2.CookieNameValue);
            Assert.NotNull(transaction5.SetCookie);
            Assert.Equal("hao", FindClaimValue(transaction5, ClaimTypes.Name));

            // Make sure new cookie is valid
            var transaction6 = await SendAsync(server, "http://example.com/me", transaction5.CookieNameValue);
            Assert.Equal("hao", FindClaimValue(transaction6, ClaimTypes.Name));
        }

        [Fact]
        public async Task CanAccessOldPrincipalDuringSecurityStampReplacement()
        {
            var clock = new TestClock();
            var server = CreateServer(services =>
            {
                services.Configure<SecurityStampValidatorOptions>(options =>
                {
                    options.OnRefreshingPrincipal = c =>
                    {
                        var newId = new ClaimsIdentity();
                        newId.AddClaim(new Claim("PreviousName", c.CurrentPrincipal.Identity.Name));
                        c.NewPrincipal.AddIdentity(newId);
                        return Task.FromResult(0);
                    };
                });
                services.AddSingleton<ISystemClock>(clock);
            });

            var transaction1 = await SendAsync(server, "http://example.com/createMe");
            Assert.Equal(HttpStatusCode.OK, transaction1.Response.StatusCode);
            Assert.Null(transaction1.SetCookie);

            var transaction2 = await SendAsync(server, "http://example.com/pwdLogin/false" );
            Assert.Equal(HttpStatusCode.OK, transaction2.Response.StatusCode);
            Assert.NotNull(transaction2.SetCookie);
            Assert.DoesNotContain("; expires=", transaction2.SetCookie);

            var transaction3 = await SendAsync(server, "http://example.com/me", transaction2.CookieNameValue);
            Assert.Equal("hao", FindClaimValue(transaction3, ClaimTypes.Name));
            Assert.Null(transaction3.SetCookie);

            // Make sure we don't get a new cookie yet
            clock.Add(TimeSpan.FromMinutes(10));
            var transaction4 = await SendAsync(server, "http://example.com/me", transaction2.CookieNameValue);
            Assert.Equal("hao", FindClaimValue(transaction4, ClaimTypes.Name));
            Assert.Null(transaction4.SetCookie);

            // Go past SecurityStampValidation interval and ensure we get a new cookie
            clock.Add(TimeSpan.FromMinutes(21));

            var transaction5 = await SendAsync(server, "http://example.com/me", transaction2.CookieNameValue);
            Assert.NotNull(transaction5.SetCookie);
            Assert.Equal("hao", FindClaimValue(transaction5, ClaimTypes.Name));
            Assert.Equal("hao", FindClaimValue(transaction5, "PreviousName"));

            // Make sure new cookie is valid
            var transaction6 = await SendAsync(server, "http://example.com/me", transaction5.CookieNameValue);
            Assert.Equal("hao", FindClaimValue(transaction6, ClaimTypes.Name));
        }

        [Fact]
        public async Task TwoFactorRememberCookieVerification()
        {
            var clock = new TestClock();
            var server = CreateServer(services => services.AddSingleton<ISystemClock>(clock));

            var transaction1 = await SendAsync(server, "http://example.com/createMe");
            Assert.Equal(HttpStatusCode.OK, transaction1.Response.StatusCode);
            Assert.Null(transaction1.SetCookie);

            var transaction2 = await SendAsync(server, "http://example.com/twofactorRememeber");
            Assert.Equal(HttpStatusCode.OK, transaction2.Response.StatusCode);

            var setCookie = transaction2.SetCookie;
            Assert.Contains(IdentityConstants.TwoFactorRememberMeScheme + "=", setCookie);
            Assert.Contains("; expires=", setCookie);

            var transaction3 = await SendAsync(server, "http://example.com/isTwoFactorRememebered", transaction2.CookieNameValue);
            Assert.Equal(HttpStatusCode.OK, transaction3.Response.StatusCode);

            // Wait for validation interval
            clock.Add(TimeSpan.FromMinutes(30));

            var transaction4 = await SendAsync(server, "http://example.com/isTwoFactorRememebered", transaction2.CookieNameValue);
            Assert.Equal(HttpStatusCode.OK, transaction4.Response.StatusCode);
        }

        [Fact]
        public async Task TwoFactorRememberCookieClearedBySecurityStampChange()
        {
            var clock = new TestClock();
            var server = CreateServer(services => services.AddSingleton<ISystemClock>(clock));

            var transaction1 = await SendAsync(server, "http://example.com/createMe");
            Assert.Equal(HttpStatusCode.OK, transaction1.Response.StatusCode);
            Assert.Null(transaction1.SetCookie);

            var transaction2 = await SendAsync(server, "http://example.com/twofactorRememeber");
            Assert.Equal(HttpStatusCode.OK, transaction2.Response.StatusCode);

            var setCookie = transaction2.SetCookie;
            Assert.Contains(IdentityConstants.TwoFactorRememberMeScheme + "=", setCookie);
            Assert.Contains("; expires=", setCookie);

            var transaction3 = await SendAsync(server, "http://example.com/isTwoFactorRememebered", transaction2.CookieNameValue);
            Assert.Equal(HttpStatusCode.OK, transaction3.Response.StatusCode);

            var transaction4 = await SendAsync(server, "http://example.com/signoutEverywhere", transaction2.CookieNameValue);
            Assert.Equal(HttpStatusCode.OK, transaction4.Response.StatusCode);

            // Doesn't validate until after interval has passed
            var transaction5 = await SendAsync(server, "http://example.com/isTwoFactorRememebered", transaction2.CookieNameValue);
            Assert.Equal(HttpStatusCode.OK, transaction5.Response.StatusCode);

            // Wait for validation interval
            clock.Add(TimeSpan.FromMinutes(30));

            var transaction6 = await SendAsync(server, "http://example.com/isTwoFactorRememebered", transaction2.CookieNameValue);
            Assert.Equal(HttpStatusCode.InternalServerError, transaction6.Response.StatusCode);
        }

        private static string FindClaimValue(Transaction transaction, string claimType)
        {
            var claim = transaction.ResponseElement.Elements("claim").SingleOrDefault(elt => elt.Attribute("type").Value == claimType);
            if (claim == null)
            {
                return null;
            }
            return claim.Attribute("value").Value;
        }

        private static async Task<XElement> GetAuthData(TestServer server, string url, string cookie)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", cookie);

            var response2 = await server.CreateClient().SendAsync(request);
            var text = await response2.Content.ReadAsStringAsync();
            var me = XElement.Parse(text);
            return me;
        }

        private static TestServer CreateServer(Action<IServiceCollection> configureServices = null, Func<HttpContext, Task> testpath = null, Uri baseAddress = null, bool testCore = false)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        var req = context.Request;
                        var res = context.Response;
                        var userManager = context.RequestServices.GetRequiredService<UserManager<PocoUser>>();
                        var roleManager = context.RequestServices.GetRequiredService<RoleManager<PocoRole>>();
                        var signInManager = context.RequestServices.GetRequiredService<SignInManager<PocoUser>>();
                        PathString remainder;
                        if (req.Path == new PathString("/normal"))
                        {
                            res.StatusCode = 200;
                        }
                        else if (req.Path == new PathString("/createMe"))
                        {
                            var user = new PocoUser("hao");
                            var result = await userManager.CreateAsync(user, TestPassword);
                            if (result.Succeeded)
                            {
                                result = await roleManager.CreateAsync(new PocoRole("role"));
                            }
                            if (result.Succeeded)
                            {
                                result = await userManager.AddToRoleAsync(user, "role");
                            }
                            res.StatusCode = result.Succeeded ? 200 : 500;
                        }
                        else if (req.Path == new PathString("/createSimple"))
                        {
                            var result = await userManager.CreateAsync(new PocoUser("simple"), "aaaaaa");
                            res.StatusCode = result.Succeeded ? 200 : 500;
                        }
                        else if (req.Path == new PathString("/signoutEverywhere"))
                        {
                            var user = await userManager.FindByNameAsync("hao");
                            var result = await userManager.UpdateSecurityStampAsync(user);
                            res.StatusCode = result.Succeeded ? 200 : 500;
                        }
                        else if (req.Path.StartsWithSegments(new PathString("/pwdLogin"), out remainder))
                        {
                            var isPersistent = bool.Parse(remainder.Value.Substring(1));
                            var result = await signInManager.PasswordSignInAsync("hao", TestPassword, isPersistent, false);
                            res.StatusCode = result.Succeeded ? 200 : 500;
                        }
                        else if (req.Path == new PathString("/twofactorRememeber"))
                        {
                            var user = await userManager.FindByNameAsync("hao");
                            await signInManager.RememberTwoFactorClientAsync(user);
                            res.StatusCode = 200;
                        }
                        else if (req.Path == new PathString("/isTwoFactorRememebered"))
                        {
                            var user = await userManager.FindByNameAsync("hao");
                            var result = await signInManager.IsTwoFactorClientRememberedAsync(user);
                            res.StatusCode = result ? 200 : 500;
                        }
                        else if (req.Path == new PathString("/hasTwoFactorUserId"))
                        {
                            var result = await context.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);
                            res.StatusCode = result.Succeeded ? 200 : 500;
                        }
                        else if (req.Path == new PathString("/me"))
                        {
                            Describe(res, AuthenticateResult.Success(new AuthenticationTicket(context.User, null, "Application")));
                        }
                        else if (req.Path.StartsWithSegments(new PathString("/me"), out remainder))
                        {
                            var auth = await context.AuthenticateAsync(remainder.Value.Substring(1));
                            Describe(res, auth);
                        }
                        else if (req.Path == new PathString("/testpath") && testpath != null)
                        {
                            await testpath(context);
                        }
                        else
                        {
                            await next();
                        }
                    });
                })
                .ConfigureServices(services =>
                {
                    if (testCore)
                    {
                        services.AddIdentityCore<PocoUser>()
                            .AddRoles<PocoRole>()
                            .AddSignInManager()
                            .AddDefaultTokenProviders();
                        services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
                    }
                    else
                    {
                        services.AddIdentity<PocoUser, PocoRole>().AddDefaultTokenProviders();
                    }
                    var store = new InMemoryStore<PocoUser, PocoRole>();
                    services.AddSingleton<IUserStore<PocoUser>>(store);
                    services.AddSingleton<IRoleStore<PocoRole>>(store);
                    configureServices?.Invoke(services);
                });
            var server = new TestServer(builder);
            server.BaseAddress = baseAddress;
            return server;
        }

        private static void Describe(HttpResponse res, AuthenticateResult result)
        {
            res.StatusCode = 200;
            res.ContentType = "text/xml";
            var xml = new XElement("xml");
            if (result != null && result.Principal != null)
            {
                xml.Add(result.Principal.Claims.Select(claim => new XElement("claim", new XAttribute("type", claim.Type), new XAttribute("value", claim.Value))));
            }
            if (result != null && result.Properties != null)
            {
                xml.Add(result.Properties.Items.Select(extra => new XElement("extra", new XAttribute("type", extra.Key), new XAttribute("value", extra.Value))));
            }
            using (var memory = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memory, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                {
                    xml.WriteTo(writer);
                }
                res.Body.Write(memory.ToArray(), 0, memory.ToArray().Length);
            }
        }

        private static async Task<Transaction> SendAsync(TestServer server, string uri, string cookieHeader = null, bool ajaxRequest = false)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            if (!string.IsNullOrEmpty(cookieHeader))
            {
                request.Headers.Add("Cookie", cookieHeader);
            }
            if (ajaxRequest)
            {
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            }
            var transaction = new Transaction
            {
                Request = request,
                Response = await server.CreateClient().SendAsync(request),
            };
            if (transaction.Response.Headers.Contains("Set-Cookie"))
            {
                transaction.SetCookie = transaction.Response.Headers.GetValues("Set-Cookie").FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(transaction.SetCookie))
            {
                transaction.CookieNameValue = transaction.SetCookie.Split(new[] { ';' }, 2).First();
            }
            transaction.ResponseText = await transaction.Response.Content.ReadAsStringAsync();

            if (transaction.Response.Content != null &&
                transaction.Response.Content.Headers.ContentType != null &&
                transaction.Response.Content.Headers.ContentType.MediaType == "text/xml")
            {
                transaction.ResponseElement = XElement.Parse(transaction.ResponseText);
            }
            return transaction;
        }

        private class Transaction
        {
            public HttpRequestMessage Request { get; set; }
            public HttpResponseMessage Response { get; set; }

            public string SetCookie { get; set; }
            public string CookieNameValue { get; set; }

            public string ResponseText { get; set; }
            public XElement ResponseElement { get; set; }
        }
    }
}
