# Dommel Extended
CRUD operations with Dapper made simple.

| Build | NuGet | MyGet | Test Coverage |
| ----- | ----- | ----- | ------------- |
| [![Travis](https://img.shields.io/travis/com/henkmollema/Dommel?style=flat-square)](https://app.travis-ci.com/github/henkmollema/Dommel) | [![NuGet](https://img.shields.io/nuget/vpre/DommelExtended.svg?style=flat-square)](https://www.nuget.org/packages/DommelExtended) | [![MyGet Pre Release](https://img.shields.io/myget/dommel-ci/vpre/Dommel.svg?style=flat-square)](https://www.myget.org/feed/dommel-ci/package/nuget/Dommel) | [![codecov](https://codecov.io/gh/henkmollema/Dommel/branch/master/graph/badge.svg)](https://codecov.io/gh/henkmollema/Dommel) |

<hr>

Dommel provides a convenient API for CRUD operations using extension methods on the `IDbConnection` interface. The SQL queries are generated based on your POCO entities. Dommel also supports LINQ expressions which are being translated to SQL expressions. [Dapper](https://github.com/StackExchange/Dapper) is used for query execution and object mapping.

Dommel Extended is just a fork aimed to provide some extended methods like bulk operations, more selects options etc.

There are several extensibility points available to change the behavior of resolving table names, column names, the key property and POCO properties. See [Extensibility](https://www.learndapper.com/extensions/dommel#extensibility) for more details.

## Installing Dommel Extended

Dommel is available on [NuGet](https://www.nuget.org/packages/Dommel).

### Install using the .NET CLI:
```
dotnet add package DommelExtended
```

### Install using the NuGet Package Manager:
```
Install-Package DommelExtended
```

## Documentation

The documentation is available at **[Learn Dapper](https://www.learndapper.com/extensions/dommel)**.


