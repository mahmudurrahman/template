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
    /// Removes schema from all entity types when running on Oracle.
    /// Oracle maps EF Core schemas to Oracle users, which requires DBA privileges to create.
    /// Stripping schemas places all tables in the connecting user's default schema.
    /// </summary>
    public static ModelBuilder RemoveSchemasForOracle(this ModelBuilder modelBuilder, DbContext context)
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