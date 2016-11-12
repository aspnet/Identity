using System;

namespace LinqToDB.Identity
{
	internal class UpdateBuilder<T, TKey>
		where T : IConcurrency<TKey>
		where TKey : IEquatable<TKey>
	{
		//private static Func<IDataContext, T, int> _update;
		//static UpdateBuilder()
		//{
		//	var dc = Expression.Parameter(typeof(IDataContext));

		//	IDataContext dc = null;
		//	var q = dc.GetTable<T>()
		//		.Where(_ => _.Id.Equals())
		//}
	}
}