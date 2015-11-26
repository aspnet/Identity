// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public abstract class ApiConsistencyTestBase
    {
        [Fact]
        public void Public_inheritable_apis_should_be_virtual()
        {
            var nonVirtualMethods
                = (from type in GetAllTypes(TargetAssembly.DefinedTypes)
                   where type.IsVisible
                         && !type.IsSealed
                         && type.DeclaredConstructors.Any(c => c.IsPublic || c.IsFamily || c.IsFamilyOrAssembly)
                         && type.Namespace != null
                         && !type.Namespace.EndsWith(".Compiled")
                   from method in type.DeclaredMethods.Where(m => m.IsPublic && !m.IsStatic)
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

        [Fact]
        public void Async_methods_should_end_with_async_suffix()
        {
            var asyncMethods
                = (from type in GetAllTypes(TargetAssembly.DefinedTypes)
                    where type.IsVisible
                    from method in type.DeclaredMethods.Where(m => m.IsPublic)
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

        protected virtual IEnumerable<TypeInfo> GetAllTypes(IEnumerable<TypeInfo> types)
        {
            foreach (var type in types)
            {
                yield return type;

                foreach (var nestedType in GetAllTypes(type.DeclaredNestedTypes))
                {
                    yield return nestedType;
                }
            }
        }

        protected TypeInfo GetBasestTypeInAssembly(Type type)
        {
            while (type.GetTypeInfo()?.BaseType?.GetTypeInfo()?.Assembly == type.GetTypeInfo().Assembly)
            {
                type = type.GetTypeInfo().BaseType;
            }

            return type.GetTypeInfo();
        }
    }
}
