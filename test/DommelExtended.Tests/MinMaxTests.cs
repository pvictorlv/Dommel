using Xunit;
using static DommelExtended.DommelMapper;

namespace DommelExtended.Tests;

public class MinMaxTests
{
    private static readonly ISqlBuilder SqlBuilder = new SqlServerSqlBuilder();

    [Fact]
    public void GeneratesMinSql()
    {
        var sql = BuildMinSql<Foo, int>(SqlBuilder, x => x.Bar, null, out var parameters);
        Assert.Equal("select min([Foos].[Bar]) from [Foos]", sql);
        Assert.Null(parameters);
    }
    
    [Fact]
    public void GeneratesMinSql_WithPredicate()
    {
        var sql = BuildMinSql<Foo, int>(SqlBuilder, x => x.Bar, x => x.Bar > 5, out var parameters);
        Assert.Equal("select min([Foos].[Bar]) from [Foos] where [Foos].[Bar] > @p1", sql);
        Assert.Single(parameters.ParameterNames);
    }

    [Fact]
    public void GeneratesMaxSql()
    {
        var sql = BuildMaxSql<Foo, int>(SqlBuilder, x => x.Bar, null, out var parameters);
        Assert.Equal("select max([Foos].[Bar]) from [Foos]", sql);
        Assert.Null(parameters);
    }
    
    [Fact]
    public void GeneratesMaxSql_WithPredicate()
    {
        var sql = BuildMaxSql<Foo, int>(SqlBuilder, x => x.Bar, x => x.Bar > 5, out var parameters);
        Assert.Equal("select max([Foos].[Bar]) from [Foos] where [Foos].[Bar] > @p1", sql);
        Assert.Single(parameters.ParameterNames);
    }

    private class Foo
    {
        public int Bar { get; set; }
    }
}
