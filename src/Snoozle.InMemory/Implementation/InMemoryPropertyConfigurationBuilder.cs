using Snoozle.Abstractions;

namespace Snoozle.InMemory.Implementation
{
    public class InMemoryPropertyConfigurationBuilder<TResource, TProperty> : BasePropertyConfigurationBuilder<TResource, TProperty, IInMemoryPropertyConfiguration>, IPropertyConfigurationBuilder<TProperty, IInMemoryPropertyConfiguration>
        where TResource : class, IRestResource
    {
        public InMemoryPropertyConfigurationBuilder(IInMemoryPropertyConfiguration propertyConfiguration)
            : base(propertyConfiguration)
        {
        }
    }
}
