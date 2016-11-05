using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace LinqToDB.Identity
{
	static class Extensions
	{
		public static T TryInsertAndSetIdentity<T>(this IDataContext db, T obj)
			where T : class
		{
			var ms = db.MappingSchema;
			var od = ms.GetEntityDescriptor(obj.GetType());

			var identity = od.Columns.FirstOrDefault(_ => _.IsIdentity);
			if (identity != null)
			{
				var res = db.InsertWithIdentity(obj);
				ms.SetValue(obj, res, identity);
			}
			else
				db.Insert(obj);

			return obj;
		}

		public static void SetValue(this MappingSchema ms, object o, object val, ColumnDescriptor column)
		{
			var ex = ms.GetConvertExpression(val.GetType(), column.MemberType);

			column.MemberAccessor.SetValue(o, ex.Compile().DynamicInvoke(val));
		}
	}
}