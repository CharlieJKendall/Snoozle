using Snoozle.Abstractions;
using System;
using System.Linq.Expressions;

namespace Snoozle
{
    public static class PropertyConfigurationBuilderExtensions
    {
        /// <summary>
        /// Set the computed value to the <see cref="DateTime.Now"/> value when the request is made.
        /// </summary>
        /// <typeparam name="TPropertyConfiguration">The type of the property configuration.</typeparam>
        /// <param name="builder">The property configuration builder instance.</param>
        /// <returns>The property configuration builder instance.</returns>
        public static IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration> DateTimeNow<TPropertyConfiguration>(
            this IComputedValueBuilder<DateTime, TPropertyConfiguration> builder)
            where TPropertyConfiguration : IPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc.ValueComputationFunc = () => DateTime.Now;

            return builder as IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration>;
        }

        /// <summary>
        /// Set the computed value to the <see cref="DateTime.UtcNow"/> value when the request is made.
        /// </summary>
        /// <typeparam name="TPropertyConfiguration">The type of the property configuration.</typeparam>
        /// <param name="builder">The property configuration builder instance.</param>
        /// <returns>The property configuration builder instance.</returns>
        public static IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration> DateTimeUtcNow<TPropertyConfiguration>(
            this IComputedValueBuilder<DateTime,
                TPropertyConfiguration> builder)
            where TPropertyConfiguration : IPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc.ValueComputationFunc = () => DateTime.UtcNow;

            return builder as IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration>;
        }

        /// <summary>
        /// Set the computed value to the specified value when the request is made.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to set the value of.</typeparam>
        /// <typeparam name="TPropertyConfiguration">The type of the property configuration.</typeparam>
        /// <param name="builder">The property configuration builder instance.</param>
        /// <param name="computationFunc">The func to apply to the property to generate the value that will be persisted.</param>
        /// <returns>The property configuration builder instance.</returns>
        public static IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> Custom<TProperty, TPropertyConfiguration>(
            this IComputedValueBuilder<TProperty, TPropertyConfiguration> builder,
            Expression<Func<TProperty>> computationFunc)
           where TPropertyConfiguration : IPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc.ValueComputationFunc = Expression.Lambda<Func<object>>(Expression.Convert(computationFunc.Body, typeof(object)));

            // Any property that has a computed value is by definition not read-only, so explicitly enforce this
            var propertyBuilder = builder as IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration>;
            propertyBuilder.IsReadOnly(false);

            return propertyBuilder;
        }
    }
}
