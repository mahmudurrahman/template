using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace FSH.Framework.Persistence;

/// <summary>
/// Extension methods for Entity Framework ModelBuilder configuration.
/// </summary>
public static class ModelBuilderExtensions
{
    private const string OracleProviderName = "Oracle.EntityFrameworkCore";

    /// <summary>
    /// Applies Oracle-specific conventions to the model:
    /// 1. Removes schemas (Oracle maps schemas to users requiring DBA privileges).
    /// 2. Remaps bool columns from BOOLEAN to NUMBER(1) for broad Oracle version compatibility.
    /// 3. Removes filtered index expressions (Oracle does not support partial indexes).
    /// 4. Makes non-PK string columns nullable (Oracle treats empty strings as NULL).
    /// </summary>
    public static ModelBuilder ApplyOracleConventions(this ModelBuilder modelBuilder, DbContext context)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Database.ProviderName?.Equals(OracleProviderName, StringComparison.OrdinalIgnoreCase) ?? true)
        {
            return modelBuilder;
        }

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.SetSchema(null);

            var primaryKeyProperties = entityType.FindPrimaryKey()?.Properties.ToHashSet() ?? [];

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(bool) || property.ClrType == typeof(bool?))
                {
                    property.SetColumnType("NUMBER(1)");
                }

                // Oracle treats '' (empty string) as NULL, so non-PK string columns
                // must be nullable to avoid ORA-01400 on empty string inserts.
                if (property.ClrType == typeof(string) && !property.IsNullable && !primaryKeyProperties.Contains(property))
                {
                    property.IsNullable = true;
                }
            }

            foreach (var index in entityType.GetIndexes())
            {
                if (index.GetFilter() is not null)
                {
                    index.SetFilter(null);
                }
            }
        }

        return modelBuilder;
    }

    /// <summary>
    /// Applies a global query filter to all entities that implement the specified interface.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to filter entities by.</typeparam>
    /// <param name="modelBuilder">The ModelBuilder instance to configure.</param>
    /// <param name="filter">The filter expression to apply to all matching entities.</param>
    /// <returns>The ModelBuilder for method chaining.</returns>
    public static ModelBuilder AppendGlobalQueryFilter<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> filter)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        ArgumentNullException.ThrowIfNull(filter);

        // get a list of entities without a baseType that implement the interface TInterface
        var entities = modelBuilder.Model.GetEntityTypes()
            .Where(e => e.BaseType is null && e.ClrType.GetInterface(typeof(TInterface).Name) is not null)
            .Select(e => e.ClrType);

        foreach (var entity in entities)
        {
            var parameterType = Expression.Parameter(modelBuilder.Entity(entity).Metadata.ClrType);
            var filterBody = ReplacingExpressionVisitor.Replace(filter.Parameters.Single(), parameterType, filter.Body);

            // get the existing query filter
            if (modelBuilder.Entity(entity).Metadata.GetQueryFilter() is { } existingFilter)
            {
                var existingFilterBody = ReplacingExpressionVisitor.Replace(existingFilter.Parameters.Single(), parameterType, existingFilter.Body);

                // combine the existing query filter with the new query filter
                filterBody = Expression.AndAlso(existingFilterBody, filterBody);
            }

            // apply the new query filter
            modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(filterBody, parameterType));
        }

        return modelBuilder;
    }
}