using Snoozle.Abstractions;

namespace Snoozle.InMemory.Implementation
{
    public interface IInMemoryModelConfiguration : IModelConfiguration
    {
        string JsonFilePath { get; set; }
    }
}