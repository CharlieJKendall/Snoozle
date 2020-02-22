namespace Snoozle.Abstractions
{
    /// <summary>
    /// Provides methods for setting the core property configuration values.
    /// </summary>
    public abstract class BasePropertyConfigurationBuilder<TResource, TProperty, TPropertyConfiguration>
        : IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration>, IComputedValueBuilder<TProperty, TPropertyConfiguration>
        where TResource : class, IRestResource
        where TPropertyConfiguration : IPropertyConfiguration
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="BasePropertyConfigurationBuilder{TResource, TProperty, TPropertyConfiguration}"/> class.
        /// </summary>
        /// <param name="propertyConfiguration">The property configuration that the builder configures.</param>
        protected BasePropertyConfigurationBuilder(TPropertyConfiguration propertyConfiguration)
        {
            PropertyConfiguration = propertyConfiguration;
        }

        public TPropertyConfiguration PropertyConfiguration { get; }

        /// <summary>
        /// Sets the property to read-only.
        /// </summary>
        public IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> IsReadOnlyColumn()
        {
            PropertyConfiguration.IsReadOnly = true;
            return this;
        }

        /// <summary>
        /// Sets the property as the primary key/identifier for the resource. Only one primary identifier can be set per resource.
        /// </summary>
        public IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> IsPrimaryIdentifier()
        {
            PropertyConfiguration.IsPrimaryResourceIdentifier = true;
            IsReadOnlyColumn();
            return this;
        }

        /// <summary>
        /// Gets a <see cref="IComputedValueBuilder{TProperty, TPropertyConfiguration}"/> that sets the value of the property automatically on write operations.
        /// </summary>
        public IComputedValueBuilder<TProperty, TPropertyConfiguration> HasComputedValue()
        {
            return this;
        }
    }
}
