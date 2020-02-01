using Microsoft.Extensions.DependencyInjection;
using Snoozle.Abstractions;
using Snoozle.Core;
using Snoozle.Extensions;
using Snoozle.SqlServer.Configuration;
using Snoozle.SqlServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Snoozle.SqlServer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IMvcBuilder AddSnoozleSqlServer(this IMvcBuilder @this)
        {
            IServiceCollection serviceCollection = @this.Services;
            ISqlRuntimeConfigurationProvider runtimeConfigurationProvider = BuildRuntimeConfigurationProvider();

            serviceCollection.AddScoped<ISqlExecutor, SqlExecutor>();
            serviceCollection.AddScoped<IDataProvider, SqlDataProvider>();
            serviceCollection.AddSingleton(runtimeConfigurationProvider);

            @this.AddSnoozleCore(runtimeConfigurationProvider);

            return @this;
        }

        public static ISqlRuntimeConfigurationProvider BuildRuntimeConfigurationProvider()
        {
            IEnumerable<ISqlResourceConfiguration> resourceConfigurations =
                Helpers.BuildResourceConfigurations<ISqlPropertyConfiguration, ISqlResourceConfiguration, ISqlModelConfiguration>(typeof(SqlResourceConfigurationBuilder<>));

            // No need to register these with the DI container, as they are only used during startup
            ISqlParamaterProvider sqlParamaterProvider = new SqlParameterProvider();
            ISqlGenerator generator = new SqlGenerator(sqlParamaterProvider);
            ISqlExpressionBuilder expressionBuilder = new SqlExpressionBuilder(sqlParamaterProvider);
            Dictionary<Type, ISqlRuntimeConfiguration<IRestResource>> runtimeConfigurations = new Dictionary<Type, ISqlRuntimeConfiguration<IRestResource>>();

            foreach (ISqlResourceConfiguration configuration in resourceConfigurations)
            {
                string selectAll = generator.SelectAll(configuration);
                string selectById = generator.SelectById(configuration);
                string deleteById = generator.DeleteById(configuration);
                string updateById = generator.Update(configuration);
                string insert = generator.Insert(configuration);

                var createObjectRelationalMapFunc = typeof(ISqlExpressionBuilder)
                    .GetMethod(nameof(ISqlExpressionBuilder.CreateObjectRelationalMap))
                    .MakeGenericMethod(configuration.ResourceType)
                    .Invoke(expressionBuilder, new[] { configuration }) as Func<SqlDataReader, IRestResource>;

                var getPrimaryKeySqlParameterFunc = expressionBuilder.GetPrimaryKeySqlParameter(configuration.PrimaryIdentifier);
                var getNonPrimaryKeySqlParametersFunc = expressionBuilder.GetNonPrimaryKeySqlParameters(configuration);

                var runtimeConfiguration = Activator.CreateInstance(
                    typeof(SqlRuntimeConfiguration<>).MakeGenericType(configuration.ResourceType),
                    configuration,
                    createObjectRelationalMapFunc,
                    getPrimaryKeySqlParameterFunc,
                    getNonPrimaryKeySqlParametersFunc,
                    selectAll,
                    selectById,
                    deleteById,
                    insert,
                    updateById) as ISqlRuntimeConfiguration<IRestResource>;

                runtimeConfigurations.Add(configuration.ResourceType, runtimeConfiguration);
            }

            return new SqlRuntimeConfigurationProvider(runtimeConfigurations);
        }
    }
}