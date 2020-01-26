using Snoozle.Core;
using Snoozle.RestResourceConfiguration;
using Snoozle.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Snoozle.Configuration
{
    public class RuntimeConfigurationProvider : IRuntimeConfigurationProvider
    {
        private readonly Dictionary<Type, IRuntimeConfiguration> _runtimeConfigurations = new Dictionary<Type, IRuntimeConfiguration>();

        public void AddRuntimeConfigurationForType(Type type, IRuntimeConfiguration configuration)
        {
            _runtimeConfigurations.Add(type, configuration);
        }

        public IRuntimeConfiguration GetRuntimeConfigurationForType(Type type)
        {
            return _runtimeConfigurations.GetValueOrDefault(type);
        }

        public static IRuntimeConfigurationProvider CreateInstance()
        {
            // Get a list containing an instance of each IResourceConfigurationBuilder implementation
            IEnumerable<IRuntimeResourceConfiguration> resourceConfigurations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .Where(restResource => restResource.BaseType != null && restResource.BaseType.IsGenericType && restResource.BaseType.GetGenericTypeDefinition().IsAssignableFrom(typeof(AbstractResourceConfigurationBuilder<>)) && !restResource.IsAbstract)
                .Select(type => ((IResourceConfigurationBuilder)Activator.CreateInstance(type)).BuildRuntimeConfiguration());

            // No need to register these with the DI container, as they are only used during startup
            ISqlParamaterProvider sqlParamaterProvider = new SqlParameterProvider();
            ISqlGenerator generator = new SqlGenerator(sqlParamaterProvider);
            ISqlExpressionBuilder expressionBuilder = new SqlExpressionBuilder(sqlParamaterProvider);
            IRuntimeConfigurationProvider configProvider = new RuntimeConfigurationProvider();

            foreach (IRuntimeResourceConfiguration config in resourceConfigurations)
            {
                string selectAll = generator.SelectAll(config);
                string selectById = generator.SelectById(config);
                string deleteById = generator.DeleteById(config);
                string updateById = generator.Update(config);
                string insert = generator.Insert(config);

                Type resourceType = config.GetType().GetGenericArguments().Single();

                var createObjectRelationalMapFunc = typeof(ISqlExpressionBuilder)
                    .GetMethod(nameof(ISqlExpressionBuilder.CreateObjectRelationalMap))
                    .MakeGenericMethod(resourceType)
                    .Invoke(expressionBuilder, new[] { config }) as Func<SqlDataReader, IRestResource>;

                var getPrimaryKeySqlParameterFunc = expressionBuilder.GetPrimaryKeySqlParameter(config.PrimaryIdentifier);

                var getNonPrimaryKeySqlParameters = typeof(ISqlExpressionBuilder)
                    .GetMethod(nameof(ISqlExpressionBuilder.GetNonPrimaryKeySqlParameters))
                    .MakeGenericMethod(resourceType)
                    .Invoke(expressionBuilder, new[] { config }) as Func<object, List<SqlParameter>>;

                var t = typeof(ISqlExpressionBuilder)
                .GetMethod(nameof(ISqlExpressionBuilder.GetPrimaryKeyValue))
                .MakeGenericMethod(resourceType)
                .Invoke(expressionBuilder, new[] { config });
                var getPrimaryKeyValue = t as Func<object, object>;

                var runtimeConfig = new RuntimeConfiguration(
                    config.AllowedVerbsFlags,
                    createObjectRelationalMapFunc,
                    getPrimaryKeySqlParameterFunc,
                    getNonPrimaryKeySqlParameters,
                    getPrimaryKeyValue,
                    selectAll,
                    selectById,
                    deleteById,
                    updateById,
                    insert,
                    config.Route);

                configProvider.AddRuntimeConfigurationForType(resourceType, runtimeConfig);
            }

            return configProvider;
        }
    }

    public interface IRuntimeConfigurationProvider
    {
        IRuntimeConfiguration GetRuntimeConfigurationForType(Type type);
        void AddRuntimeConfigurationForType(Type type, IRuntimeConfiguration configuration);
    }
}
