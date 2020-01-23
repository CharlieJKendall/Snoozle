using Snoozle.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Snoozle.Configuration
{
    public class RuntimeConfiguration : IRuntimeConfiguration
    {
        public RuntimeConfiguration(
            Func<SqlDataReader, IRestResource> getSqlMapToResource,
            Func<object, SqlParameter> getPrimaryKeySqlParameter,
            string selectAllSql,
            string selectByIdSql,
            string deleteByIdSql)
        {
            GetSqlMapToResource = getSqlMapToResource;
            GetPrimaryKeySqlParameter = getPrimaryKeySqlParameter;
            SqlStrings = new SqlStrings(selectAllSql, selectByIdSql, deleteByIdSql);
        }        

        public Func<SqlDataReader, IRestResource> GetSqlMapToResource { get; }
        public Func<object, SqlParameter> GetPrimaryKeySqlParameter { get; }
        public SqlStrings SqlStrings { get; }
    }

    public interface IRuntimeConfiguration
    {
        Func<SqlDataReader, IRestResource> GetSqlMapToResource { get; }
        Func<object, SqlParameter> GetPrimaryKeySqlParameter { get; }
        SqlStrings SqlStrings { get; }
    }
}
