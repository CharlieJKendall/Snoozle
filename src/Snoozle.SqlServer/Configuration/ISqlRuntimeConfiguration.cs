using Snoozle.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Snoozle.SqlServer.Configuration
{
    public interface ISqlRuntimeConfiguration<out TResource> : IRuntimeConfiguration
        where TResource : class, IRestResource
    {
        Func<SqlDataReader, TResource> GetSqlMapToResource { get; }
        Func<object, SqlParameter> GetPrimaryKeySqlParameter { get; }
        Func<object, List<SqlParameter>> GetNonPrimaryKeySqlParameters { get; }

        string SelectAll { get; }
        string SelectById { get; }
        string DeleteById { get; }
        string Insert { get; }
        string UpdateById { get; }
    }
}