using Snoozle.Abstractions;
using Snoozle.Abstractions.Models;
using Snoozle.Exceptions;
using System;
using System.Linq.Expressions;

namespace Snoozle
{
    public static class PropertyConfigurationBuilderExtensions
    {
        public static IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration> DateTimeNow<TPropertyConfiguration>(
            this IComputedValueBuilder<DateTime, TPropertyConfiguration> builder,
            HttpVerb endpointTriggers = HttpVerb.POST | HttpVerb.PUT)
            where TPropertyConfiguration : IPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc = new ValueComputationFuncModel(() => DateTime.Now, endpointTriggers);

            return builder as IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration>;
        }

        public static IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration> DateTimeUtcNow<TPropertyConfiguration>(
            this IComputedValueBuilder<DateTime,
                TPropertyConfiguration> builder,
                HttpVerb endpointTriggers = HttpVerb.POST | HttpVerb.PUT)
            where TPropertyConfiguration : IPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc = new ValueComputationFuncModel(() => DateTime.UtcNow, endpointTriggers);

            return builder as IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration>;
        }

        public static IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> Custom<TProperty, TPropertyConfiguration>(
            this IComputedValueBuilder<TProperty, TPropertyConfiguration> builder,
            Expression<Func<object>> computationFunc,
            HttpVerb endpointTriggers = HttpVerb.POST | HttpVerb.PUT)
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
