using Snoozle.Abstractions;

namespace Snoozle.InMemory.Implementation
{
    public class InMemoryModelConfiguration<TResource> : BaseModelConfiguration<TResource>, IInMemoryModelConfiguration
        where TResource : class, IRestResource
    {
        public string JsonFilePath { get; set; }
    }
}
