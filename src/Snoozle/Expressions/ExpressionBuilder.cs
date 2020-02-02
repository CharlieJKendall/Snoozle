using Snoozle.Abstractions;
using Snoozle.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Snoozle.Expressions
{
    public static class ExpressionBuilder
    {
        public static Func<object, object> GetPrimaryKeyValueFunc<TResource, TPropertyConfiguration, TModelConfiguration>(
            IResourceConfiguration<TPropertyConfiguration, TModelConfiguration> config)
            where TResource : class, IRestResource
            where TPropertyConfiguration : class, IPropertyConfiguration
            where TModelConfiguration : class, IModelConfiguration
        {
            var paramResource = Expression.Parameter(typeof(object), "resourceObject");
            var property = Expression.Convert(
                Expression.Property(Expression.Convert(paramResource, typeof(TResource)), config.PrimaryIdentifier.PropertyName),
                typeof(object));

            var lambda = Expression.Lambda<Func<object, object>>(
                property,
                paramResource);

            return lambda.Compile();
        }

        public static IEnumerable<ValueComputationActionModel> GetComputedValueAction<TResource, TPropertyConfiguration, TModelConfiguration>(
            IResourceConfiguration<TPropertyConfiguration, TModelConfiguration> resourceConfiguration)
            where TResource : class, IRestResource
            where TPropertyConfiguration : class, IPropertyConfiguration
            where TModelConfiguration : class, IModelConfiguration
        {
            var configs = resourceConfiguration.PropertyConfigurationsForWrite.Where(c => c.ValueComputationFunc != null).ToArray();

            var paramResource = Expression.Parameter(typeof(object), "resourceObject");
            var typedResource = Expression.Variable(typeof(TResource), "typedResource");
            var assignTyped = Expression.Assign(typedResource, Expression.Convert(paramResource, typeof(TResource)));
            var actionModels = new ValueComputationActionModel[configs.Length];

            for (int i = 0; i < configs.Length; i++)
            {
                var property = Expression.Property(assignTyped, configs[i].PropertyName);
                var assignProperty = Expression.Assign(
                        Expression.MakeMemberAccess(assignTyped, property.Member),
                        Expression.Convert(configs[i].ValueComputationFunc.ValueComputationFunc.Body, property.Type));


                var lambda = Expression.Lambda<Action<object>>(
                    Expression.Block(new[] { typedResource }, assignTyped, assignProperty),
                    paramResource);

                var compiled = lambda.Compile();

                actionModels[i] = new ValueComputationActionModel(compiled, configs[i].ValueComputationFunc.EndpointTriggers);
            }

            return actionModels;
        }
    }
}
