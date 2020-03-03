using Snoozle.Abstractions;
using System.Collections.Generic;

namespace Snoozle.InMemory.Implementation
{
    public class InMemoryResourceConfiguration<TResource> : BaseResourceConfiguration<TResource, IInMemoryPropertyConfiguration, IInMemoryModelConfiguration>, IInMemoryResourceConfiguration
        where TResource : class, IRestResource
    {
        public InMemoryResourceConfiguration(
            IInMemoryModelConfiguration modelConfiguration,
            IEnumerable<IInMemoryPropertyConfiguration> propertyConfigurations)
            : base(modelConfiguration, propertyConfigurations)
        {
        }
    }
}
