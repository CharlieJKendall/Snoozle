using Snoozle.Core;
using Snoozle.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Snoozle.Configuration
{
    public class RuntimeConfiguration : IRuntimeConfiguration
    {
        public RuntimeConfiguration(
            HttpVerb allowedVerbsFlag,
            Func<SqlDataReader, IRestResource> getSqlMapToResource,
            Func<object, SqlParameter> getPrimaryKeySqlParameter,
            Func<object, List<SqlParameter>> getNonPrimaryKeySqlParameters,
            Func<object, object> getPrimaryKeyValue,
            string selectAllSql,
            string selectByIdSql,
            string deleteByIdSql,
            string updateByIdSql,
            string insertSql,
            string route)
        {
            AllowedVerbsFlags = allowedVerbsFlag;
            GetSqlMapToResource = getSqlMapToResource;
            GetPrimaryKeySqlParameter = getPrimaryKeySqlParameter;
            GetNonPrimaryKeySqlParameters = getNonPrimaryKeySqlParameters;
            GetPrimaryKeyValue = getPrimaryKeyValue;
            SqlStrings = new SqlStrings(selectAllSql, selectByIdSql, deleteByIdSql, insertSql, updateByIdSql);
            Route = route;
        }        

        public Func<SqlDataReader, IRestResource> GetSqlMapToResource { get; }
        public Func<object, SqlParameter> GetPrimaryKeySqlParameter { get; }
        public Func<object, List<SqlParameter>> GetNonPrimaryKeySqlParameters { get; }
        public Func<object, object> GetPrimaryKeyValue { get; }
        public SqlStrings SqlStrings { get; }
        public HttpVerb AllowedVerbsFlags { get; }
        public string Route { get; }
    }

    public interface IRuntimeConfiguration
    {
        Func<SqlDataReader, IRestResource> GetSqlMapToResource { get; }
        Func<object, SqlParameter> GetPrimaryKeySqlParameter { get; }
        Func<object, object> GetPrimaryKeyValue { get; }
        Func<object, List<SqlParameter>> GetNonPrimaryKeySqlParameters { get; }
        SqlStrings SqlStrings { get; }
        HttpVerb AllowedVerbsFlags { get; }
        string Route { get; }
    }
}
