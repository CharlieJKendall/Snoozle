using Microsoft.Extensions.Logging;
using Snoozle.Abstractions;
using Snoozle.SqlServer.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snoozle.SqlServer.Implementation
{
    public class SqlDataProvider : IDataProvider
    {
        private readonly ISqlRuntimeConfigurationProvider _sqlRuntimeConfigurationProvider;
        private readonly ISqlExecutor _sqlExecutor;
        private readonly ILogger<IDataProvider> _logger;

        public SqlDataProvider(ISqlRuntimeConfigurationProvider sqlRuntimeConfigurationProvider, ISqlExecutor sqlExecutor, ILogger<IDataProvider> logger)
        {
            _sqlRuntimeConfigurationProvider = sqlRuntimeConfigurationProvider;
            _sqlExecutor = sqlExecutor;
            _logger = logger;
        }

        public async Task<bool> ExecuteDeleteByIdAsync<TResource>(object primaryKey)
            where TResource : class, IRestResource
        {
            var config = GetConfig<TResource>();

            return await _sqlExecutor.ExecuteDeleteByIdAsync(
                config.DeleteById,
                config.GetPrimaryKeySqlParameter,
                primaryKey);
        }

        public async Task<TResource> ExecuteInsertAsync<TResource>(TResource resourceToCreate)
            where TResource : class, IRestResource
        {
            var config = GetConfig<TResource>();

            return await _sqlExecutor.ExecuteInsertAsync(
                config.Insert,
                config.GetNonPrimaryKeySqlParameters,
                config.GetSqlMapToResource,
                resourceToCreate);
        }

        public async Task<IEnumerable<TResource>> ExecuteSelectAllAsync<TResource>()
            where TResource : class, IRestResource
        {
            var config = GetConfig<TResource>();

            return await _sqlExecutor.ExecuteSelectAllAsync(config.SelectAll, config.GetSqlMapToResource);
        }

        public async Task<TResource> ExecuteSelectByIdAsync<TResource>(object primaryKey)
            where TResource : class, IRestResource
        {
            var config = GetConfig<TResource>();

            try
            {
                return await _sqlExecutor.ExecuteSelectByIdAsync(
                    config.SelectById,
                    config.GetSqlMapToResource,
                    config.GetPrimaryKeySqlParameter,
                    primaryKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find resource of type {Type} with a primary key value of {PrimaryKey}", typeof(TResource).Name, primaryKey);

                return null;
            }
        }

        public async Task<TResource> ExecuteUpdateAsync<TResource>(TResource resourceToCreate, object primaryKey)
            where TResource : class, IRestResource
        {
            var config = GetConfig<TResource>();

            return await _sqlExecutor.ExecuteUpdateAsync(
                config.UpdateById,
                config.GetNonPrimaryKeySqlParameters,
                config.GetPrimaryKeySqlParameter,
                config.GetSqlMapToResource,
                resourceToCreate,primaryKey);
        }

        private ISqlRuntimeConfiguration<TResource> GetConfig<TResource>()
            where TResource : class, IRestResource
        {
            return (ISqlRuntimeConfiguration<TResource>)_sqlRuntimeConfigurationProvider.GetRuntimeConfigurationForType(typeof(TResource));
        }
    }
}
