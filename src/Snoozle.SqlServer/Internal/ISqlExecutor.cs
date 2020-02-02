using Snoozle.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Snoozle.SqlServer.Internal
{
    public interface ISqlExecutor
    {
        Task<T> ExecuteSelectByIdAsync<T>(
            string sql,
            Func<SqlDataReader, T> mappingFunc,
            Func<object, SqlParameter> paramProvider,
            object primaryKey) where T : class, IRestResource;

        Task<IEnumerable<T>> ExecuteSelectAllAsync<T>(
            string sql,
            Func<SqlDataReader, T> mappingFunc) where T : class, IRestResource;

        Task<bool> ExecuteDeleteByIdAsync(
           string sql,
           Func<object, SqlParameter> paramProvider,
           object primaryKey);

        Task<T> ExecuteInsertAsync<T>(
            string sql,
            Func<object, List<SqlParameter>> paramProvider,
            Func<SqlDataReader, T> mappingFunc,
            T resource) where T : class, IRestResource;

        Task<T> ExecuteUpdateAsync<T>(
            string sql,
            Func<object, List<SqlParameter>> paramProvider,
            Func<object, SqlParameter> primaryKeyParamProvider,
            Func<SqlDataReader, T> mappingFunc,
            T resourceToCreate,
            object primaryKey) where T : class, IRestResource;
    }
}