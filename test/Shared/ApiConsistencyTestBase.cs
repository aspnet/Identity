// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Tests
{
    public abstract class ApiConsistencyTestBase
    {
        protected const BindingFlags PublicInstance
            = BindingFlags.Instance | BindingFlags.Public;

        protected const BindingFlags AnyInstance
            = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [Fact]
        public void Public_inheritable_apis_should_be_virtual()
        {
            var nonVirtualMethods
                = (from type in GetAllTypes(TargetAssembly.GetTypes())
                   where type.IsVisible
                         && !type.IsSealed
                         && type.GetConstructors(AnyInstance).Any(c => c.IsPublic || c.IsFamily || c.IsFamilyOrAssembly)
                         && type.Namespace != null
                         && !type.Namespace.EndsWith(".Compiled")
                   from method in type.GetMethods(PublicInstance)
                   where GetBasestTypeInAssembly(method.DeclaringType) == type
                         && !(method.IsVirtual && !method.IsFinal) 
                         && !method.Name.StartsWith("get_") 
                         && !method.Name.StartsWith("set_")
                         && !method.Name.Equals("Dispose")
                   select type.Name + "." + method.Name)
                    .ToList();

            Assert.False(
                nonVirtualMethods.Any(),
                "\r\n-- Missing virtual APIs --\r\n" + string.Join("\r\n", nonVirtualMethods));
        }

        //[Fact]
        //public void Public_api_arguments_should_have_not_null_annotation()
        //{
        //    var parametersMissingAttribute
        //        = (from type in GetAllTypes(TargetAssembly.GetTypes())
        //            where type.IsVisible && !typeof(Delegate).IsAssignableFrom(type)
        //            let interfaceMappings = type.GetInterfaces().Select(type.GetInterfaceMap)
        //            let events = type.GetEvents()
        //            from method in type.GetMethods(PublicInstance | BindingFlags.Static)
        //                .Concat<MethodBase>(type.GetConstructors())
        //            where GetBasestTypeInAssembly(method.DeclaringType) == type
        //            where type.IsInterface || !interfaceMappings.Any(im => im.TargetMethods.Contains(method))
        //            where !events.Any(e => e.AddMethod == method || e.RemoveMethod == method)
        //            from parameter in method.GetParameters()
        //            where !parameter.ParameterType.IsValueType
        //                  && !parameter.GetCustomAttributes()
        //                      .Any(
        //                          a => a.GetType().Name == "NotNullAttribute"
        //                               || a.GetType().Name == "CanBeNullAttribute")
        //            select type.Name + "." + method.Name + "[" + parameter.Name + "]")
        //            .ToList();

        //    Assert.False(
        //        parametersMissingAttribute.Any(),
        //        "\r\n-- Missing NotNull annotations --\r\n" + string.Join("\r\n", parametersMissingAttribute));
        //}

        [Fact]
        public void Async_methods_should_end_with_async_suffix()
        {
            var asyncMethods
                = (from type in GetAllTypes(TargetAssembly.GetTypes())
                    where type.IsVisible
                    from method in type.GetMethods(PublicInstance/* | BindingFlags.Static*/)
                    where GetBasestTypeInAssembly(method.DeclaringType) == type
                    where typeof(Task).IsAssignableFrom(method.ReturnType)
                    select method).ToList();

            var missingSuffixMethods
                = asyncMethods
                    .Where(method => !method.Name.EndsWith("Async"))
                    .Select(method => method.DeclaringType.Name + "." + method.Name)
                    .Except(GetAsyncSuffixExceptions())
                    .ToList();

            Assert.False(
                missingSuffixMethods.Any(),
                "\r\n-- Missing async suffix --\r\n" + string.Join("\r\n", missingSuffixMethods));
        }

        protected virtual IEnumerable<string> GetCancellationTokenExceptions()
        {
            return Enumerable.Empty<string>();
        }

        protected virtual IEnumerable<string> GetAsyncSuffixExceptions()
        {
            return Enumerable.Empty<string>();
        }

        protected abstract Assembly TargetAssembly { get; }

        protected virtual IEnumerable<Type> GetAllTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                yield return type;

                foreach (var nestedType in GetAllTypes(type.GetNestedTypes()))
                {
                    yield return nestedType;
                }
            }
        }

        protected Type GetBasestTypeInAssembly(Type type)
        {
            while (type.BaseType != null
                   && type.BaseType.Assembly == type.Assembly)
            {
                type = type.BaseType;
            }

            return type;
        }
    }
}
