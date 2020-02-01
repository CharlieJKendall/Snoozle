using Snoozle.Abstractions;

namespace Snoozle.SqlServer.Configuration
{
    public interface ISqlRuntimeConfigurationProvider : IRuntimeConfigurationProvider<ISqlRuntimeConfiguration<IRestResource>>
    {
    }
}
