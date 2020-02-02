using Snoozle.Abstractions.Models;
using Snoozle.Expressions;
using System;
using System.Collections.Generic;

namespace Snoozle.Abstractions
{
    public abstract class BaseRuntimeConfiguration<TPropertyConfiguration, TModelConfiguration, TResource> : IRuntimeConfiguration
        where TPropertyConfiguration : class, IPropertyConfiguration
        where TModelConfiguration : class, IModelConfiguration
        where TResource : class, IRestResource
    {
        public Func<object, object> GetPrimaryKeyValue { get; }

        public HttpVerb AllowedVerbsFlags { get; }

        public string Route { get; }

        public IEnumerable<ValueComputationActionModel> ValueComputationActions { get; }

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
