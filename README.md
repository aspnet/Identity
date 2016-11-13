[LinqToDB](https://github.com/linq2db/linq2db) Identity store provider for [ASP.NET Core Identity](https://github.com/aspnet/Identity)
===

AppVeyor last build: [![Build status](https://ci.appveyor.com/api/projects/status/x15cyc688w9247oj?svg=true)](https://ci.appveyor.com/project/ili/linqtodb-identity)

AppVeyor master build: [![Build status](https://ci.appveyor.com/api/projects/status/x15cyc688w9247oj/branch/master?svg=true)](https://ci.appveyor.com/project/ili/linqtodb-identity/branch/master)

## NuGet
* [NuGet.org](https://www.nuget.org/packages/LinqToDB.Identity/)
* [MyGet.org](https://www.myget.org/feed/ili/package/nuget/LinqToDB.Identity) feed: https://www.myget.org/F/ili/api/v3/index.json


## Usage
In general this is the same as for Entity Framework, just call `AddLinqToDBStores` instead of `AddEntityFrameworkStores` in your `Startup.cs` like [here](https://github.com/ili/LinqToDB.Identity/blob/master/samples/IdentitySample.Mvc/Startup.cs#L62)

The main difference with Entity Framework Core storage provider are:
* We do not use hardcoded classes - interfaces like `IIdentityUser<TKey>` are used (but yes, we do have default implementation)
* Data connection factory is used for calling to database

## Special
All source code is based on original Microsoft Entity Framework Core storage provider for [ASP.NET Core Identity](https://github.com/aspnet/Identity).

Tests and sample are just adopted for using [LinqToDB](https://github.com/linq2db/linq2db). For inmemory storage tests SQLite inmemory database is used.
