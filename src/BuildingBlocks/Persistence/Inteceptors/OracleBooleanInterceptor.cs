using System.Data.Common;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ERA.Framework.Persistence.Interceptors;

/// <summary>
/// Intercepts Oracle SQL commands to replace boolean literals (True/False)
/// with numeric equivalents (1/0), as Oracle does not support boolean literals in SQL.
/// This works around an Oracle EF Core provider limitation where LINQ translations
/// like AnyAsync generate "THEN True ELSE False" instead of "THEN 1 ELSE 0".
/// </summary>
public sealed partial class OracleBooleanInterceptor : DbCommandInterceptor
{
    [GeneratedRegex(@"\bTrue\b", RegexOptions.Compiled)]
    private static partial Regex TrueRegex();

    [GeneratedRegex(@"\bFalse\b", RegexOptions.Compiled)]
    private static partial Regex FalseRegex();

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        ArgumentNullException.ThrowIfNull(command);
        FixBooleanLiterals(command);
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        FixBooleanLiterals(command);
        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(command);
        FixBooleanLiterals(command);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        FixBooleanLiterals(command);
        return new ValueTask<InterceptionResult<int>>(result);
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        ArgumentNullException.ThrowIfNull(command);
        FixBooleanLiterals(command);
        return result;
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        FixBooleanLiterals(command);
        return new ValueTask<InterceptionResult<object>>(result);
    }

#pragma warning disable CA2100 // SQL is from EF Core's query pipeline, not user input
    private static void FixBooleanLiterals(DbCommand command)
    {
        string sql = command.CommandText;
        if (sql.Contains("True", StringComparison.Ordinal) || sql.Contains("False", StringComparison.Ordinal))
        {
            sql = TrueRegex().Replace(sql, "1");
            sql = FalseRegex().Replace(sql, "0");
            command.CommandText = sql;
        }
    }
#pragma warning restore CA2100
}
