using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;

namespace DommelExtended;

public static partial class DommelMapper
{
    /// <summary>
    /// Selects the maximum value of the specified column.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="selector">The column to select the maximum value of.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The maximum value of the specified column.</returns>
    public static TResult Max<TEntity, TResult>(this IDbConnection connection, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, IDbTransaction? transaction = null)
    {
        var sql = BuildMaxSql(GetSqlBuilder(connection), selector, predicate, out var parameters);
        LogQuery<TEntity>(sql);
        return connection.ExecuteScalar<TResult>(sql, parameters, transaction)!;
    }

    /// <summary>
    /// Selects the maximum value of the specified column.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="selector">The column to select the maximum value of.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The maximum value of the specified column.</returns>
    public static Task<TResult> MaxAsync<TEntity, TResult>(this IDbConnection connection, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null, IDbTransaction? transaction = null)
    {
        var sql = BuildMaxSql(GetSqlBuilder(connection), selector, predicate, out var parameters);
        LogQuery<TEntity>(sql);
        return connection.ExecuteScalarAsync<TResult>(sql, parameters, transaction)!;
    }

    public static string BuildMaxSql<TEntity, TResult>(ISqlBuilder sqlBuilder, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate, out DynamicParameters? parameters)
    {
        var tableName = Resolvers.Table(typeof(TEntity), sqlBuilder);
        var columnName = GetColumnName(sqlBuilder, selector);
        var sql = $"select max({columnName}) from {tableName}";

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
}
