using System.Data.SqlClient;

namespace Snoozle.SqlServer.Internal
{
    public class SqlClassProvider : ISqlClassProvider
    {
        public SqlCommand CreateSqlCommand(string sql, SqlConnection sqlConnection)
        {
            return new SqlCommand(sql, sqlConnection);
        }

        public SqlConnection CreateSqlConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
