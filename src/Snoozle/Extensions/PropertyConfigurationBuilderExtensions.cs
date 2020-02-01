using Snoozle.Abstractions;
using System;

namespace Snoozle.Extensions
{
    public static class PropertyConfigurationBuilderExtensions
    {
        public static IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration> DateTimeNow<TPropertyConfiguration>(this IComputedValueBuilder<DateTime, TPropertyConfiguration> builder)
            where TPropertyConfiguration : IPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc = () => DateTime.Now;
            return builder as IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration>;
        }

        public static IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration> DateTimeUtcNow<TPropertyConfiguration>(this IComputedValueBuilder<DateTime, TPropertyConfiguration> builder)
            where TPropertyConfiguration : IPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc = () => DateTime.UtcNow;
            return builder as IPropertyConfigurationBuilder<DateTime, TPropertyConfiguration>;
        }

        public static IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> Custom<TProperty, TPropertyConfiguration>(this IComputedValueBuilder<TProperty, TPropertyConfiguration> builder, Func<TProperty> computationFunc)
           where TPropertyConfiguration : IPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc = ConvertToFuncObject(computationFunc);
            return builder as IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration>;
        }

        public static Func<object> ConvertToFuncObject<T>(Func<T> func)
        {
            return () => func();
        }
    }
}
