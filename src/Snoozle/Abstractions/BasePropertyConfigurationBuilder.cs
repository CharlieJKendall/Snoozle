namespace Snoozle.Abstractions
{
    public abstract class BasePropertyConfigurationBuilder<TResource, TProperty, TPropertyConfiguration>
        : IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration>, IComputedValueBuilder<TProperty, TPropertyConfiguration>
        where TResource : class, IRestResource
        where TPropertyConfiguration : IPropertyConfiguration
    {
        protected BasePropertyConfigurationBuilder(TPropertyConfiguration propertyConfiguration)
        {
            PropertyConfiguration = propertyConfiguration;
        }

        public TPropertyConfiguration PropertyConfiguration { get; }

        public IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> IsReadOnlyColumn()
        {
            PropertyConfiguration.IsReadOnly = true;
            return this;
        }

        public IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> IsPrimaryIdentifier()
        {
            PropertyConfiguration.IsPrimaryResourceIdentifier = true;
            IsReadOnlyColumn();
            return this;
        }

        public IComputedValueBuilder<TProperty, TPropertyConfiguration> HasComputedValue()
        {
            return this;
        }
    }
}
