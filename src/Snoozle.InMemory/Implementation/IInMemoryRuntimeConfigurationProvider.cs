using Snoozle.Abstractions;

namespace Snoozle.InMemory.Implementation
{
    public interface IInMemoryRuntimeConfigurationProvider : IRuntimeConfigurationProvider<IInMemoryRuntimeConfiguration<IRestResource>>
    {
    }
}
