// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	///     Contains extension methods to <see cref="IdentityBuilder" /> for adding entity framework stores.
	/// </summary>
	public static class IdentityLinqToDbBuilderExtensions
	{
		/// <summary>
		///     Adds an Entity Framework implementation of identity information stores.
		/// </summary>
		/// <typeparam name="TContext">
		///     The type of the class for <see cref="IDataContext" />,
		///     <see cref="IConnectionFactory{TContext,TConnection}" />
		/// </typeparam>
		/// <typeparam name="TConnection">
		///     The type of the class for <see cref="DataConnection" />,
		///     <see cref="IConnectionFactory{TContext,TConnection}" />
		/// </typeparam>
		/// <param name="builder">The <see cref="IdentityBuilder" /> instance this method extends.</param>
		/// <param name="factory">
		///     <see cref="IConnectionFactory{TContext,TConnection}" />
		/// </param>
		/// <returns>The <see cref="IdentityBuilder" /> instance this method extends.</returns>
		// ReSharper disable once InconsistentNaming
		public static IdentityBuilder AddLinqToDBStores<TContext, TConnection>(this IdentityBuilder builder,
			IConnectionFactory<TContext, TConnection> factory)
			where TContext : IDataContext
			where TConnection : DataConnection
		{
			builder.Services.AddSingleton(factory);

			builder.Services.TryAdd(GetDefaultServices(builder.UserType, builder.RoleType, typeof(TContext), typeof(TConnection)));
			return builder;
		}

		/// <summary>
		///     Adds an Entity Framework implementation of identity information stores.
		/// </summary>
		/// <typeparam name="TContext">
		///     The type of the class for <see cref="IDataContext" />,
		///     <see cref="IConnectionFactory{TContext,TConnection}" />
		/// </typeparam>
		/// <typeparam name="TConnection">
		///     The type of the class for <see cref="DataConnection" />,
		///     <see cref="IConnectionFactory{TContext,TConnection}" />
		/// </typeparam>
		/// <typeparam name="TKey">The type of the primary key used for the users and roles.</typeparam>
		/// <param name="builder">The <see cref="IdentityBuilder" /> instance this method extends.</param>
		/// <param name="factory">
		///     <see cref="IConnectionFactory{TContext,TConnection}" />
		/// </param>
		/// <returns>The <see cref="IdentityBuilder" /> instance this method extends.</returns>
		// ReSharper disable once InconsistentNaming
		public static IdentityBuilder AddLinqToDBStores<TContext, TConnection, TKey>(this IdentityBuilder builder,
			IConnectionFactory<TContext, TConnection> factory)
			where TContext : IDataContext
			where TConnection : DataConnection
			where TKey : IEquatable<TKey>
		{
			builder.Services.AddSingleton(factory);

			builder.Services.TryAdd(GetDefaultServices(builder.UserType, builder.RoleType, typeof(TContext), typeof(TKey)));
			return builder;
		}

		private static IServiceCollection GetDefaultServices(Type userType, Type roleType, Type contextType,
			Type connectionType, Type keyType = null)
		{
			Type userStoreType;
			Type roleStoreType;
			keyType = keyType ?? typeof(string);
			userStoreType = typeof(UserStore<,,,,>).MakeGenericType(contextType, connectionType, userType, roleType, keyType);
			roleStoreType = typeof(RoleStore<,,,>).MakeGenericType(contextType, connectionType, roleType, keyType);

			var services = new ServiceCollection();
			services.AddScoped(
				typeof(IUserStore<>).MakeGenericType(userType),
				userStoreType);
			services.AddScoped(
				typeof(IRoleStore<>).MakeGenericType(roleType),
				roleStoreType);
			return services;
		}
	}
}