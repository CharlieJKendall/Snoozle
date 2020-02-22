using Snoozle.Abstractions;
using Snoozle.Abstractions.Models;
using Snoozle.Exceptions;
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
        /// <param name="endpointTriggers">The endpoints that will trigger this computed value.</param>
        /// <returns>The property configuration builder instance.</returns>
        public static IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration> DateTimeNow<TPropertyConfiguration>(
            this IComputedValueBuilder<DateTime, TPropertyConfiguration> builder,
            HttpVerbs endpointTriggers = HttpVerbs.POST | HttpVerbs.PUT)
            where TPropertyConfiguration : IPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc = new ValueComputationFuncModel(() => DateTime.Now, endpointTriggers);

            return builder as IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration>;
        }

        /// <summary>
        /// Set the computed value to the <see cref="DateTime.UtcNow"/> value when the request is made.
        /// </summary>
        /// <typeparam name="TPropertyConfiguration">The type of the property configuration.</typeparam>
        /// <param name="builder">The property configuration builder instance.</param>
        /// <param name="endpointTriggers">The endpoints that will trigger this computed value.</param>
        /// <returns>The property configuration builder instance.</returns>
        public static IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration> DateTimeUtcNow<TPropertyConfiguration>(
            this IComputedValueBuilder<DateTime,
                TPropertyConfiguration> builder,
                HttpVerbs endpointTriggers = HttpVerbs.POST | HttpVerbs.PUT)
            where TPropertyConfiguration : IPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc = new ValueComputationFuncModel(() => DateTime.UtcNow, endpointTriggers);

            return builder as IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration>;
        }

        /// <summary>
        /// Set the computed value to the specified value when the request is made.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to set the value of.</typeparam>
        /// <typeparam name="TPropertyConfiguration">The type of the property configuration.</typeparam>
        /// <param name="builder">The property configuration builder instance.</param>
        /// <param name="computationFunc">The func to apply to the property to generate the value that will be persisted.</param>
        /// <param name="endpointTriggers">The endpoints that will trigger this computed value.</param>
        /// <returns>The property configuration builder instance.</returns>
        public static IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> Custom<TProperty, TPropertyConfiguration>(
            this IComputedValueBuilder<TProperty, TPropertyConfiguration> builder,
            Expression<Func<object>> computationFunc,
            HttpVerbs endpointTriggers = HttpVerbs.POST | HttpVerbs.PUT)
           where TPropertyConfiguration : IPropertyConfiguration
        {
            ExceptionHelper.Argument.ThrowIfTrue(
                computationFunc.Body.Type != typeof(TProperty),
                $"Computation Func return type must match property type ({typeof(TProperty).Name}).",
                nameof(computationFunc));

            builder.PropertyConfiguration.ValueComputationFunc = new ValueComputationFuncModel(computationFunc, endpointTriggers); ;

            return builder as IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration>;
        }
    }
}
