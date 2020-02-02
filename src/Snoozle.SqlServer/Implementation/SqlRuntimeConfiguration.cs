using Snoozle.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Snoozle.SqlServer.Implementation
{
    public class SqlRuntimeConfiguration<TResource> : BaseRuntimeConfiguration<ISqlPropertyConfiguration, ISqlModelConfiguration>, ISqlRuntimeConfiguration<TResource>
        where TResource : class, IRestResource
    {
        // This is created by reflection, so be careful when changing/adding parameters
        public SqlRuntimeConfiguration(
            ISqlResourceConfiguration resourceConfiguration,
            Func<SqlDataReader, TResource> getSqlMapToResource,
            Func<object, SqlParameter> getPrimaryKeySqlParameter,
            Func<object, List<SqlParameter>> getNonPrimaryKeySqlParameters,
            string selectAll,
            string selectById,
            string deleteById,
            string insert,
            string updateById)
            : base(resourceConfiguration)
        {
            GetSqlMapToResource = getSqlMapToResource;
            GetPrimaryKeySqlParameter = getPrimaryKeySqlParameter;
            GetNonPrimaryKeySqlParameters = getNonPrimaryKeySqlParameters;
            SelectAll = selectAll;
            SelectById = selectById;
            DeleteById = deleteById;
            Insert = insert;
            UpdateById = updateById;
        }

        public Func<SqlDataReader, TResource> GetSqlMapToResource { get; }
        public Func<object, SqlParameter> GetPrimaryKeySqlParameter { get; }
        public Func<object, List<SqlParameter>> GetNonPrimaryKeySqlParameters { get; }
        public string SelectAll { get; }
        public string SelectById { get; }
        public string DeleteById { get; }
        public string Insert { get; }
        public string UpdateById { get; }
    }
}
