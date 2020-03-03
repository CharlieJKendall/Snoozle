using Snoozle.Abstractions;

namespace Snoozle.InMemory.Implementation
{
    public interface IInMemoryPropertyConfiguration : IPropertyConfiguration
    {
        int GetNextPrimaryKeyValue();
    }
}
