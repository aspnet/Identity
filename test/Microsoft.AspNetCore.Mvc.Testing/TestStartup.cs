using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    public class TestStartup<TStartup> where TStartup : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TestServiceRegistrations _registrations;
        private readonly TStartup _instance;

        public TestStartup(IServiceProvider serviceProvider, TestServiceRegistrations registrations)
        {
            _serviceProvider = serviceProvider;
            _registrations = registrations;
            _instance = (TStartup)ActivatorUtilities.CreateInstance(serviceProvider, typeof(TStartup));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configureServices = _instance.GetType().GetMethod(nameof(ConfigureServices));
            var parameters = Enumerable.Repeat(services, 1)
                .Concat(configureServices
                    .GetParameters()
                    .Skip(1)
                    .Select(p => ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, p.ParameterType)))
                .ToArray();

            _registrations.ConfigureServices(services, () => configureServices.Invoke(_instance, parameters));
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            var configure = _instance.GetType().GetMethod(nameof(Configure));
            var parameters = Enumerable.Repeat(applicationBuilder, 1)
                .Concat(configure
                    .GetParameters()
                    .Skip(1)
                    .Select(p => ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, p.ParameterType)))
                .ToArray();

            configure.Invoke(_instance, parameters);
        }
    }
}
