using System.Threading.Tasks;
using IdentityOIDCWebApplicationSample.Identity.Data;
using IdentityOIDCWebApplicationSample.Identity.Models;
using IdentityOIDCWebApplicationSample.Identity.Services;
using Microsoft.AspNetCore.Applications.Authentication.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Service.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityOIDCWebApplicationSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IdentityApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var builder = services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddApplications<IdentityClientApplication>()
                .AddEntityFrameworkStores<IdentityApplicationDbContext>()
                .AddDeveloperCertificate()
                .AddClientExtensions();

            services.AddSingleton<IAuthorizationHandler, ApplicationManagementHandler>();

            services.ConfigureApplicationTokens(options =>
                options.Issuer = "https://localhost/DFC7191F-FF74-42B9-A292-08FEA80F5B20/v2.0/");

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            });

            services.AddAuthentication()
                .AddOpenIdConnect(options => options.ClientId = "56A33E6A-ADFE-47EA-BBFE-40F4AE4C55BA")
                .WithIntegratedWebClient()
                .AddCookie();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseDevelopmentCertificateErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRewriter(new RewriteOptions().AddRedirectToHttps(StatusCodes.Status302Found, 44324));

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public class ApplicationManagementHandler : AuthorizationHandler<ApplicationManagementRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApplicationManagementRequirement requirement)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
    }
}
