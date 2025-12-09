using Xunit;
using DommelExtended;

namespace DommelExtended.Tests.SqlExpressions;

public class GroupByHavingTests
{
    private readonly SqlExpression<Foo> _sqlExpression = new(new SqlServerSqlBuilder());

    [Fact]
    public void GroupBy()
    {
        var sql = _sqlExpression
            .Select()
            .GroupBy(x => x.Bar)
            .ToSql();
        Assert.Equal("select * from [Foos] group by [Foos].[Bar]", sql);
    }

    [Fact]
    public void GroupBy_Multiple()
    {
        var sql = _sqlExpression
            .Select()
            .GroupBy(x => new { x.Bar, x.Baz })
            .ToSql();
        Assert.Equal("select * from [Foos] group by [Foos].[Bar], [Foos].[Baz]", sql);
    }
    
    [Fact]
    public void Having()
    {
        var sql = _sqlExpression
            .Select()
            .GroupBy(x => x.Bar)
            .Having(x => x.Bar > 5)
            .ToSql();
        Assert.Equal("select * from [Foos] group by [Foos].[Bar] having [Foos].[Bar] > @p1", sql);
    }
    
    [Fact]
    public void Having_And()
    {
        var sql = _sqlExpression
            .Select()
            .GroupBy(x => x.Bar)
            .Having(x => x.Bar > 5)
            .AndHaving(x => x.Baz == "a")
            .ToSql();
        Assert.Equal("select * from [Foos] group by [Foos].[Bar] having ([Foos].[Bar] > @p1) and ([Foos].[Baz] = @p2)", sql);
    }
    
    [Fact]
    public void Having_Or()
    {
        var sql = _sqlExpression
            .Select()
            .GroupBy(x => x.Bar)
            .Having(x => x.Bar > 5)
            .OrHaving(x => x.Baz == "a")
            .ToSql();
        Assert.Equal("select * from [Foos] group by [Foos].[Bar] having ([Foos].[Bar] > @p1) or ([Foos].[Baz] = @p2)", sql);
    }

    private class Foo
    {
        public int Bar { get; set; }
        public string? Baz { get; set; }
    }
}
