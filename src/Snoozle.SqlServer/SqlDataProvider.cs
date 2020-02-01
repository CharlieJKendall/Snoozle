using Snoozle.Abstractions;
using Snoozle.SqlServer.Configuration;
using Snoozle.SqlServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snoozle.SqlServer
{
    public class SqlDataProvider : IDataProvider
    {
        private readonly ISqlRuntimeConfigurationProvider _sqlRuntimeConfigurationProvider;
        private readonly ISqlExecutor _sqlExecutor;

        public SqlDataProvider(ISqlRuntimeConfigurationProvider sqlRuntimeConfigurationProvider, ISqlExecutor sqlExecutor)
        {
            _sqlRuntimeConfigurationProvider = sqlRuntimeConfigurationProvider;
            _sqlExecutor = sqlExecutor;
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
            return _sqlRuntimeConfigurationProvider.GetRuntimeConfigurationForType(typeof(TResource)) as ISqlRuntimeConfiguration<TResource>;
        }
    }
}
