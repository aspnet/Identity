// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test.Utilities;

namespace Microsoft.AspNetCore.Identity.EntityFrameworkCore.Test
{
    public class ScratchDatabaseFixture : IDisposable
    {
        private SqlServerTestStore _testStore;
	    private SqlServerTestStore TestStore => _testStore ?? (_testStore = SqlServerTestStore.CreateScratch());

        public string ConnectionString => TestStore.Connection.ConnectionString;

        public void Dispose()
        {
	        _testStore?.Dispose();
        }
    }
}