using System;
using System.Collections.Generic;
using Microsoft.AspNet.DependencyInjection;
using Microsoft.AspNet.DependencyInjection.Fallback;

namespace Microsoft.AspNet.Identity.Test
{
    public static class TestServices
    {
        public static IServiceProvider DefaultServiceProvider<TUser, TKey>()
            where TUser : class,IUser<TKey>
            where TKey : IEquatable<TKey>
        {
            var container = new ServiceCollection();
            container.Add(TestServices.DefaultServices<TUser, TKey>());
            return container.BuildServiceProvider();
        }

        public static IEnumerable<IServiceDescriptor> DefaultServices<TUser, TKey>() 
            where TUser : class,IUser<TKey>
            where TKey : IEquatable<TKey>
        {
            return new IServiceDescriptor[]
            {
                new ServiceDescriptor<IPasswordValidator, PasswordValidator>(),
                new ServiceDescriptor<IUserValidator<TUser, TKey>, UserValidator<TUser, TKey>>(),
                new ServiceDescriptor<IPasswordHasher, PasswordHasher>(),
            };
        }

        public class ServiceDescriptor<TService, TImplementation> : IServiceDescriptor
        {
            public ServiceDescriptor(LifecycleKind lifecycle = LifecycleKind.Transient)
            {
                Lifecycle = lifecycle;
            }

            public LifecycleKind Lifecycle { get; private set; }

            public Type ServiceType
            {
                get { return typeof (TService); }
            }

            public Type ImplementationType
            {
                get { return typeof (TImplementation); }
            }

            public object ImplementationInstance
            {
                get { return null; }
            }
        }

        public class ServiceInstanceDescriptor<TService> : IServiceDescriptor
        {
            public ServiceInstanceDescriptor(object instance)
            {
                ImplementationInstance = instance;
            }

            public LifecycleKind Lifecycle
            {
                get { return LifecycleKind.Singleton; }
            }

            public Type ServiceType
            {
                get { return typeof (TService); }
            }

            public Type ImplementationType
            {
                get { return null; }
            }

            public object ImplementationInstance { get; private set; }
        }
    }
}