using Microsoft.Extensions.Options;
using Snoozle.Abstractions;
using Snoozle.SqlServer.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Snoozle.SqlServer.Internal
{
    public class SqlExecutor : ISqlExecutor
    {
        private readonly string _connectionString;
        private readonly ISqlClassProvider _sqlClassProvider;

        public SqlExecutor(IOptions<SnoozleSqlServerOptions> options, ISqlClassProvider sqlClassProvider)
        {
            _connectionString = options.Value.ConnectionString;
            _sqlClassProvider = sqlClassProvider;
        }

        public async Task<IEnumerable<T>> ExecuteSelectAllAsync<T>(string sql, Func<SqlDataReader, T> mappingFunc)
            where T : class, IRestResource
        {
            using (var connection = _sqlClassProvider.CreateSqlConnection(_connectionString))
            using (SqlCommand command = _sqlClassProvider.CreateSqlCommand(sql, connection))
            {
                await connection.OpenAsync();
                List<T> results = new List<T>();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(mappingFunc(reader));
                    }
                }

                return results;
            }
        }

        public async Task<T> ExecuteSelectByIdAsync<T>(
            string sql,
            Func<SqlDataReader, T> mappingFunc,
            Func<object, SqlParameter> paramProvider,
            object primaryKey)
            where T : class, IRestResource
        {
            using (SqlConnection connection = _sqlClassProvider.CreateSqlConnection(_connectionString))
            using (SqlCommand command = _sqlClassProvider.CreateSqlCommand(sql, connection))
            {
                command.Parameters.Add(paramProvider(primaryKey));

                await connection.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return mappingFunc(reader);
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }

        public async Task<bool> ExecuteDeleteByIdAsync(
           string sql,
           Func<object, SqlParameter> paramProvider,
           object primaryKey)
        {
            using (SqlConnection connection = _sqlClassProvider.CreateSqlConnection(_connectionString))
            using (SqlCommand command = _sqlClassProvider.CreateSqlCommand(sql, connection))
            {
                command.Parameters.Add(paramProvider(primaryKey));

                await connection.OpenAsync();

                int numberOfRowsDeleted = await command.ExecuteNonQueryAsync();

                return numberOfRowsDeleted != 0;
            }
        }

        public async Task<T> ExecuteInsertAsync<T>(
            string sql,
            Func<object, List<SqlParameter>> paramProvider,
            Func<SqlDataReader, T> mappingFunc,
            T resourceToCreate)
            where T : class, IRestResource
        {
            using (SqlConnection connection = _sqlClassProvider.CreateSqlConnection(_connectionString))
            using (SqlCommand command = _sqlClassProvider.CreateSqlCommand(sql, connection))
            {
                command.Parameters.AddRange(paramProvider(resourceToCreate).ToArray());

                await connection.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return mappingFunc(reader);
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }

        public async Task<T> ExecuteUpdateAsync<T>(
            string sql,
            Func<object, List<SqlParameter>> paramProvider,
            Func<object, SqlParameter> primaryKeyParamProvider,
            Func<SqlDataReader, T> mappingFunc,
            T resourceToCreate,
            object primaryKey)
            where T : class, IRestResource
        {
            using (SqlConnection connection = _sqlClassProvider.CreateSqlConnection(_connectionString))
            using (SqlCommand command = _sqlClassProvider.CreateSqlCommand(sql, connection))
            {
                command.Parameters.AddRange(paramProvider(resourceToCreate).ToArray());
                command.Parameters.Add(primaryKeyParamProvider(primaryKey));

                await connection.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return mappingFunc(reader);
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }
    }
}
