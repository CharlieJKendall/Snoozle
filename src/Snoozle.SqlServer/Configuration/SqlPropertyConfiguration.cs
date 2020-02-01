using Snoozle.Abstractions;
using System.Data;

namespace Snoozle.SqlServer.Configuration
{
    public class SqlPropertyConfiguration : BasePropertyConfiguration, ISqlPropertyConfiguration
    {
        public SqlDbType? SqlDbType { get; set; }

        public string ColumnName { get; set; }
    }
}
