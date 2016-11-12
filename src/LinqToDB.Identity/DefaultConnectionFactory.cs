using LinqToDB.Data;

namespace LinqToDB.Identity
{
	/// <summary>
	///     Represents default <see cref="IConnectionFactory{TContext,TConnection}" />
	/// </summary>
	/// <typeparam name="TContext">The type of the data getContext class used to access the store.</typeparam>
	/// <typeparam name="TConnection">The type repewsenting database getConnection <see cref="DataConnection" /></typeparam>
	public class DefaultConnectionFactory<TContext, TConnection> : IConnectionFactory<TContext, TConnection>
		where TContext : class, IDataContext, new()
		where TConnection : DataConnection, new()
	{
		/// <summary>
		///     Creates <see cref="DataConnection" /> with default parameters
		/// </summary>
		/// <returns>
		///     <see cref="DataConnection" />
		/// </returns>
		public TConnection GetConnection() => new TConnection();

		/// <summary>
		///     Creates <see cref="DataContext" /> with default parameters
		/// </summary>
		/// <returns>
		///     <see cref="DataContext" />
		/// </returns>
		public TContext GetContext() => new TContext();
	}
}