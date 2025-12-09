using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;

namespace Dommel;

public static partial class DommelMapper
{
    /// <summary>
    /// Selects the minimum value of the specified column.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="selector">The column to select the minimum value of.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The minimum value of the specified column.</returns>
    public static TResult Min<TEntity, TResult>(this IDbConnection connection, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, IDbTransaction? transaction = null)
    {
        var sql = BuildMinSql(GetSqlBuilder(connection), selector, predicate, out var parameters);
        LogQuery<TEntity>(sql);
        return connection.ExecuteScalar<TResult>(sql, parameters, transaction)!;
    }

    /// <summary>
    /// Selects the minimum value of the specified column.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="selector">The column to select the minimum value of.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The minimum value of the specified column.</returns>
    public static Task<TResult> MinAsync<TEntity, TResult>(this IDbConnection connection, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, IDbTransaction? transaction = null)
    {
        var sql = BuildMinSql(GetSqlBuilder(connection), selector, predicate, out var parameters);
        LogQuery<TEntity>(sql);
        return connection.ExecuteScalarAsync<TResult>(sql, parameters, transaction)!;
    }

    internal static string BuildMinSql<TEntity, TResult>(ISqlBuilder sqlBuilder, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate, out DynamicParameters? parameters)
    {
        var tableName = Resolvers.Table(typeof(TEntity), sqlBuilder);
        var columnName = GetColumnName(sqlBuilder, selector);
        var sql = $"select min({columnName}) from {tableName}";

        if (predicate != null)
        {
            sql += CreateSqlExpression<TEntity>(sqlBuilder)
                .Where(predicate)
                .ToSql(out var paramsRef);
            parameters = paramsRef;
        }
        else
        {
            parameters = null;
        }

        return sql;
    }

    private static string GetColumnName<TEntity, TResult>(ISqlBuilder sqlBuilder, Expression<Func<TEntity, TResult>> selector)
    {
        MemberExpression? memberExpression = null;

        if (selector.Body.NodeType == ExpressionType.Convert)
        {
            var body = (UnaryExpression)selector.Body;
            memberExpression = body.Operand as MemberExpression;
        }
        else if (selector.Body.NodeType == ExpressionType.MemberAccess)
        {
            memberExpression = selector.Body as MemberExpression;
        }

        if (memberExpression == null)
        {
            throw new ArgumentException("Expression must be a member access", nameof(selector));
        }

        var property = memberExpression.Member as PropertyInfo;
        if (property == null)
        {
            throw new ArgumentException("Expression must be a property", nameof(selector));
        }

        return Resolvers.Column(property, sqlBuilder);
    }
}
