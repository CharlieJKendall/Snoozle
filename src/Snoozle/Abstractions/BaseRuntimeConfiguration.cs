using Snoozle.Enums;
using Snoozle.Expressions;
using System;

namespace Snoozle.Abstractions
{
    public abstract class BaseRuntimeConfiguration<TPropertyConfiguration, TModelConfiguration> : IRuntimeConfiguration
        where TPropertyConfiguration : class, IPropertyConfiguration
        where TModelConfiguration : class, IModelConfiguration
    {
        protected BaseRuntimeConfiguration(IResourceConfiguration<TPropertyConfiguration, TModelConfiguration> configuration)
        {
            GetPrimaryKeyValue = CreatePrimaryKeyLambda(configuration);
            AllowedVerbsFlags = configuration.AllowedVerbsFlags;
            Route = configuration.Route;
        }

        private Func<object, object> CreatePrimaryKeyLambda(IResourceConfiguration<TPropertyConfiguration, TModelConfiguration> configuration)
        {
            return typeof(ExpressionBuilder)
                .GetMethod(nameof(ExpressionBuilder.GetPrimaryKeyValue))
                .MakeGenericMethod(configuration.ResourceType, typeof(TPropertyConfiguration), typeof(TModelConfiguration))
                .Invoke(null, new[] { configuration }) as Func<object, object>;
        }

        public Func<object, object> GetPrimaryKeyValue { get; }

        public HttpVerb AllowedVerbsFlags { get; }

        public string Route { get; }
    }
}
