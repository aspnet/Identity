using LinqToDB.Data;

namespace LinqToDB.Identity
{
	/// <summary>
	///     Represents connection factory
	/// </summary>
	/// <typeparam name="TContext">
	///     <see cref="IDataContext" />
	/// </typeparam>
	/// <typeparam name="TConnection">
	///     <see cref="DataConnection" />
	/// </typeparam>
	public interface IConnectionFactory<out TContext, out TConnection>
		where TContext : IDataContext
		where TConnection : DataConnection
	{
		/// <summary>
		///     Gets new instance of <see cref="IDataContext" />
		/// </summary>
		/// <returns>
		///     <see cref="IDataContext" />
		/// </returns>
		TContext GetContext();

		/// <summary>
		///     Gets new instance of <see cref="DataConnection" />
		/// </summary>
		/// <returns>
		///     <see cref="DataConnection" />
		/// </returns>
		TConnection GetConnection();
	}
}