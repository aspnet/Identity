using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    public class MvcWebApplicationBuilder<TStartup> where TStartup : class
    {
        public string ContentRoot { get; set; }
        public IList<Action<IServiceCollection>> ConfigureServicesBeforeStartup { get; set; } = new List<Action<IServiceCollection>>();
        public IList<Action<IServiceCollection>> ConfigureServicesAfterStartup { get; set; } = new List<Action<IServiceCollection>>();
        public List<Assembly> ApplicationAssemblies { get; set; } = new List<Assembly>();

        public MvcWebApplicationBuilder<TStartup> ConfigureBeforeStartup(Action<IServiceCollection> configure)
        {
            ConfigureServicesBeforeStartup.Add(configure);
            return this;
        }

        public MvcWebApplicationBuilder<TStartup> ConfigureAfterStartup(Action<IServiceCollection> configure)
        {
            ConfigureServicesAfterStartup.Add(configure);
            return this;
        }

        public MvcWebApplicationBuilder<TStartup> UseApplicationAssemblies()
        {
            ApplicationAssemblies.AddRange(DefaultAssemblyPartDiscoveryProvider
                .DiscoverAssemblyParts(typeof(TStartup).Assembly.GetName().Name)
                .Select(s => ((AssemblyPart)s).Assembly)
                .ToList());

            return this;
        }

        public MvcWebApplicationBuilder<TStartup> UseSolutionRelativeContentRoot(
            string solutionRelativePath,
            string solutionName = "*.sln")
        {
            var applicationBasePath = AppContext.BaseDirectory;

            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var solutionPath = Directory.EnumerateFiles(directoryInfo.FullName, solutionName).FirstOrDefault();
                if (solutionPath != null)
                {
                    ContentRoot = Path.GetFullPath(Path.Combine(directoryInfo.FullName, solutionRelativePath));
                    return this;
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Solution root could not be located using application root {applicationBasePath}.");
        }

        public TestServer Build()
        {
            using (new CultureReplacer())
            {
                var builder = new WebHostBuilder()
                    .UseContentRoot(ContentRoot)
                    .ConfigureServices(InitializeServices);

                builder.UseStartup<TestStartup<TStartup>>();

                return new TestServer(builder);
            }
        }

        protected virtual void InitializeServices(IServiceCollection services)
        {
            // Inject a custom application part manager. Overrides AddMvcCore() because that uses TryAdd().
            var manager = new ApplicationPartManager();
            foreach (var assembly in ApplicationAssemblies)
            {
                manager.ApplicationParts.Add(new AssemblyPart(assembly));
            }

            services.AddSingleton(manager);
            services.AddSingleton(new TestServiceRegistrations
            {
                Before = ConfigureServicesBeforeStartup,
                After = ConfigureServicesAfterStartup
            });
        }
    }
}
