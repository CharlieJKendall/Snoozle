using Snoozle.Abstractions.Models;
using Snoozle.Expressions;
using System;
using System.Collections.Generic;

namespace Snoozle.Abstractions
{
    /// <summary>
    /// Base class representing the configuration values stored in a singleton and accessed at runtime.
    /// </summary>
    public abstract class BaseRuntimeConfiguration<TPropertyConfiguration, TModelConfiguration, TResource> : IRuntimeConfiguration
        where TPropertyConfiguration : class, IPropertyConfiguration
        where TModelConfiguration : class, IModelConfiguration
        where TResource : class, IRestResource
    {
        /// <summary>
        /// A func that gets the primary key value given a resource object.
        /// </summary>
        public Func<object, object> GetPrimaryKeyValue { get; }

        /// <summary>
        /// The allowed HTTP method verbs for this resource.
        /// </summary>
        public HttpVerb AllowedVerbsFlags { get; }

        /// <summary>
        /// The API route that the resource can be accessed at.
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// The computation actions to be applied to any properties on write operations.
        /// </summary>
        public IEnumerable<ValueComputationActionModel> ValueComputationActions { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="BaseRuntimeConfiguration{TPropertyConfiguration, TModelConfiguration, TResource}"/> class.
        /// </summary>
        /// <param name="configuration">The resource configuration.</param>
        protected BaseRuntimeConfiguration(IResourceConfiguration<TPropertyConfiguration, TModelConfiguration> configuration)
        {
            GetPrimaryKeyValue = CreatePrimaryKeyFunc(configuration);
            ValueComputationActions = CreateValueComputationActions(configuration);
            AllowedVerbsFlags = configuration.AllowedVerbsFlags;
            Route = configuration.Route;
        }

        private IEnumerable<ValueComputationActionModel> CreateValueComputationActions(IResourceConfiguration<TPropertyConfiguration, TModelConfiguration> configuration)
        {
            return typeof(ExpressionBuilder)
                .GetMethod(nameof(ExpressionBuilder.GetComputedValueAction))
                .MakeGenericMethod(configuration.ResourceType, typeof(TPropertyConfiguration), typeof(TModelConfiguration))
                .Invoke(null, new[] { configuration }) as IEnumerable<ValueComputationActionModel>;
        }

        private Func<object, object> CreatePrimaryKeyFunc(IResourceConfiguration<TPropertyConfiguration, TModelConfiguration> configuration)
        {
            return typeof(ExpressionBuilder)
                .GetMethod(nameof(ExpressionBuilder.GetPrimaryKeyValueFunc))
                .MakeGenericMethod(configuration.ResourceType, typeof(TPropertyConfiguration), typeof(TModelConfiguration))
                .Invoke(null, new[] { configuration }) as Func<object, object>;
        }
    }
}
