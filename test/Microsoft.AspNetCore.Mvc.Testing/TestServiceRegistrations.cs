using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    public class TestServiceRegistrations
    {
        public IList<Action<IServiceCollection>> Before { get; set; } = new List<Action<IServiceCollection>>();
        public IList<Action<IServiceCollection>> After { get; set; } = new List<Action<IServiceCollection>>();

        public void ConfigureServices(IServiceCollection services, Action startupConfigureServices)
        {
            foreach (var config in Before)
            {
                config(services);
            }

            startupConfigureServices();

            foreach (var config in After)
            {
                config(services);
            }
        }
    }
}
