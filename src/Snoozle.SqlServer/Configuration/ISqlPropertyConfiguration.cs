using Snoozle.Abstractions;
using System.Data;

namespace Snoozle.SqlServer.Configuration
{
    public interface ISqlPropertyConfiguration : IPropertyConfiguration
    {
        SqlDbType? SqlDbType { get; set; }

        string ColumnName { get; set; }
    }
}
