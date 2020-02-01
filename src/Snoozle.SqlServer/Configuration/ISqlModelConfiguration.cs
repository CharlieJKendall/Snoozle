using Snoozle.Abstractions;

namespace Snoozle.SqlServer.Configuration
{
    public interface ISqlModelConfiguration : IModelConfiguration
    {
        string TableName { get; set; }
    }
}