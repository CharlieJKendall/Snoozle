using System;
using System.Collections.Generic;
using System.Linq;

namespace Snoozle.Abstractions
{
    /// <summary>
    /// Provides the core configuration values for a REST resource.
    /// </summary>
    public abstract class BaseResourceConfiguration<TResource, TPropertyConfiguration, TModelConfiguration> : IResourceConfiguration<TPropertyConfiguration, TModelConfiguration>
        where TResource : class, IRestResource
        where TPropertyConfiguration : class, IPropertyConfiguration
        where TModelConfiguration : class, IModelConfiguration
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="BaseRuntimeConfiguration{TPropertyConfiguration, TModelConfiguration, TResource}"/> class.
        /// </summary>
        protected BaseResourceConfiguration(TModelConfiguration modelConfiguration, IEnumerable<TPropertyConfiguration> propertyConfigurations)
        {
            ModelConfiguration = modelConfiguration;
            AllPropertyConfigurations = propertyConfigurations;
        }

        /// <summary>
        /// All property configurations for values to be returned during read operations.
        /// </summary>
        public IEnumerable<TPropertyConfiguration> PropertyConfigurationsForRead => AllPropertyConfigurations.OrderBy(prop => prop.Index);

        /// <summary>
        /// The property configuration of the primary key/identifier for the resource.
        /// </summary>
        public TPropertyConfiguration PrimaryIdentifier => AllPropertyConfigurations.Single(prop => prop.IsPrimaryResourceIdentifier);

        /// <summary>
        /// All property configurations for values to be set during creation operations.
        /// </summary>
        public IEnumerable<TPropertyConfiguration> PropertyConfigurationsForCreate => AllPropertyConfigurations.OrderBy(prop => prop.Index).Where(prop => !prop.IsReadOnly);


        /// <summary>
        /// All property configurations for values to be set during update operations.
        /// </summary>
        public IEnumerable<TPropertyConfiguration> PropertyConfigurationsForUpdate =>
            PropertyConfigurationsForCreate.Where(x => (!x.HasComputedValue || x.ValueComputationFunc.HasEndpointTrigger(HttpVerbs.PUT)) && !x.IsPrimaryResourceIdentifier);

        /// <summary>
        /// Allowed HTTP method verbs for the resource. This overrides the globally configured verbs.
        /// </summary>
        public HttpVerbs AllowedVerbsFlags => ModelConfiguration.AllowedVerbsFlags;

        /// <summary>
        /// The API route that the resource can be accessed at.
        /// </summary>
        public string Route => ModelConfiguration.Route;

        /// <summary>
        /// The <see cref="Type"/> of the resource.
        /// </summary>
        public Type ResourceType => typeof(TResource);

        /// <summary>
        /// The model configuration for the resource.
        /// </summary>
        public TModelConfiguration ModelConfiguration { get; }

        /// <summary>
        /// The property configurations for the resource.
        /// </summary>
        public IEnumerable<TPropertyConfiguration> AllPropertyConfigurations { get; }
    }
}
