using Snoozle.Abstractions;
using System;
using System.Linq.Expressions;

namespace Snoozle.Expressions
{
    public static class ExpressionBuilder
    {
        public static Func<object, object> GetPrimaryKeyValue<TResource, TPropertyConfiguration, TModelConfiguration>(IResourceConfiguration<TPropertyConfiguration, TModelConfiguration> config)
            where TResource : class, IRestResource
            where TPropertyConfiguration : class, IPropertyConfiguration
            where TModelConfiguration : class, IModelConfiguration
        {
            var paramResource = Expression.Parameter(typeof(object), "resource");
            var property = Expression.Convert(
                Expression.Property(Expression.Convert(paramResource, typeof(TResource)), config.PrimaryIdentifier.PropertyName),
                typeof(object));

            var lambda = Expression.Lambda<Func<object, object>>(
                property,
                paramResource);

            return lambda.Compile();
        }
    }
}
