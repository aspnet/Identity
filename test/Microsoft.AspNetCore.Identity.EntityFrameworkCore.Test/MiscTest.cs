using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public class MiscTest
    {
		[Fact]
		public void SqlServerConnectionStringBuilderTest()
		{
			var csb =
				new SqlConnectionStringBuilder(
					@"Server = (local)\SQL2012SP1; Database = master; User ID = sa; Password = Password12!")
				{
					InitialCatalog = "demo1"
				};

			var pwd = csb.Password;

			Assert.Equal("Password12!", pwd);
			Assert.Contains("Password12!", csb.ConnectionString);

			csb.InitialCatalog = "demo";

			Assert.Equal("Password12!", pwd);
			Assert.Contains("Password12!", csb.ConnectionString);
		}
	}
}
