using Microsoft.Extensions.DependencyInjection;
using Snoozle.Configuration;
using Snoozle.Core;
using Snoozle.RestResourceConfiguration;
using Snoozle.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Snoozle.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseSnoozle(this IServiceCollection @this)
        {
            @this.AddScoped<ISqlExecutor, SqlExecutor>();
            @this.AddScoped<IRuntimeConfigurationProvider, StaticRuntimeConfigurationProvider>();

            // Get a list containing an instance of each AbstractResourceConfigurationBuilder implementation
            IEnumerable<IRuntimeResourceConfiguration<IRestResource>> resourceConfigurations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .Where(restResource => restResource.BaseType != null && restResource.BaseType.IsGenericType && restResource.BaseType.GetGenericTypeDefinition().IsAssignableFrom(typeof(AbstractResourceConfigurationBuilder<>)) && !restResource.IsAbstract)
                .Select(type => ((IResourceConfigurationBuilder<IRestResource>)Activator.CreateInstance(type)).BuildRuntimeConfiguration());

            ISqlGenerator generator = new SqlGenerator();
            ISqlExpressionBuilder expressionBuilder = new SqlExpressionBuilder();

            // This can only be done because we use a static config provider implementation. This should be resolved from the DI container if we move away from this
            IRuntimeConfigurationProvider configProvider = new StaticRuntimeConfigurationProvider();

            foreach (IRuntimeResourceConfiguration<IRestResource> config in resourceConfigurations)
            {
                string selectAll = generator.SelectAll(config);
                string selectById = generator.SelectById(config);
                string deleteById = generator.DeleteById(config);

                Type resourceType = config.GetType().GetGenericArguments().Single();

                var createObjectRelationalMapFunc = typeof(ISqlExpressionBuilder)
                    .GetMethod(nameof(ISqlExpressionBuilder.CreateObjectRelationalMapFunc))
                    .MakeGenericMethod(resourceType)
                    .Invoke(expressionBuilder, new[] { config }) as Func<SqlDataReader, IRestResource>;

                var getPrimaryKeySqlParameterFunc = expressionBuilder.GetPrimaryKeySqlParameter(config.PrimaryIdentifier).Compile();

                var runtimeConfig = new RuntimeConfiguration(createObjectRelationalMapFunc, getPrimaryKeySqlParameterFunc, selectAll, selectById, deleteById);

                configProvider.AddRuntimeConfigurationForType(resourceType, runtimeConfig);
            }

            return @this;
        }
    }
}
