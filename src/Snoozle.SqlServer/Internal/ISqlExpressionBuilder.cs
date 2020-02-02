using Snoozle.Abstractions;
using Snoozle.SqlServer.Implementation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Snoozle.SqlServer.Internal
{
    public interface ISqlExpressionBuilder
    {
        Func<object, SqlParameter> GetPrimaryKeySqlParameter(ISqlPropertyConfiguration primaryIdentifierConfig);

        Func<SqlDataReader, T> CreateObjectRelationalMap<T>(ISqlResourceConfiguration config)
            where T : class, IRestResource;

        Func<object, List<SqlParameter>> GetNonPrimaryKeySqlParameters(ISqlResourceConfiguration config);
    }
}
