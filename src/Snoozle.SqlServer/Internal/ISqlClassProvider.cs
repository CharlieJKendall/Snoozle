using System.Data.SqlClient;

namespace Snoozle.SqlServer.Internal
{
    public interface ISqlClassProvider
    {
        SqlCommand CreateSqlCommand(string sql, SqlConnection sqlConnection);

        SqlConnection CreateSqlConnection(string connectionString);
    }
}
