using System;
using System.Collections.Generic;
using System.Linq;

namespace Snoozle.Abstractions
{
    public abstract class BaseResourceConfiguration<TResource, TPropertyConfiguration, TModelConfiguration> : IResourceConfiguration<TPropertyConfiguration, TModelConfiguration>
        where TResource : class, IRestResource
        where TPropertyConfiguration : class, IPropertyConfiguration
        where TModelConfiguration : class, IModelConfiguration
    {
        protected BaseResourceConfiguration(TModelConfiguration modelConfiguration, IEnumerable<TPropertyConfiguration> propertyConfigurations)
        {
            ModelConfiguration = modelConfiguration;
            AllPropertyConfigurations = propertyConfigurations;
        }

        public IEnumerable<TPropertyConfiguration> PropertyConfigurationsForRead => AllPropertyConfigurations.OrderBy(prop => prop.Index);

        public TPropertyConfiguration PrimaryIdentifier => AllPropertyConfigurations.Single(prop => prop.IsPrimaryResourceIdentifier);

        public IEnumerable<TPropertyConfiguration> PropertyConfigurationsForWrite => AllPropertyConfigurations.OrderBy(prop => prop.Index).Where(prop => !prop.IsReadOnly);

        public HttpVerb AllowedVerbsFlags => ModelConfiguration.AllowedVerbsFlags;

        public string Route => ModelConfiguration.Route;

        public Type ResourceType => typeof(TResource);

        public TModelConfiguration ModelConfiguration { get; }

        public IEnumerable<TPropertyConfiguration> AllPropertyConfigurations { get; }
    }
}
