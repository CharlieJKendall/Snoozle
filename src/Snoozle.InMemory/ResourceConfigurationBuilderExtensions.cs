using Snoozle.Abstractions;
using Snoozle.Exceptions;
using Snoozle.InMemory.Implementation;
using System;
using System.Threading;

namespace Snoozle.InMemory
{
    public static class ModelConfigurationBuilderExtensions
    {
        /// <summary>
        /// Path to the JSON file that contains the initial data for the REST API.
        /// </summary>
        public static IModelConfigurationBuilder<IInMemoryModelConfiguration> HasJsonFilePath(
            this IModelConfigurationBuilder<IInMemoryModelConfiguration> @this,
            string jsonFilePath)
        {
            ExceptionHelper.ArgumentNull.ThrowIfNecessary(jsonFilePath, nameof(jsonFilePath));

            @this.ModelConfiguration.JsonFilePath = jsonFilePath;

            return @this;
        }
    }

    public static class PropertyConfigurationBuilderExtensions
    {
        private static int _globalPrimaryKey = 0;
        private static readonly object _lock = new object();

        /// <summary>
        /// Sets the minimum value of the auto-incremented primary key to be used globally by all resources if the provided value is larger than the
        /// existing minimum value. This should only be called during app startup.
        /// </summary>
        public static void SetGlobalPrimaryKeyInteger(int value)
        {
            lock (_lock)
            {
                if (value > _globalPrimaryKey)
                {
                    _globalPrimaryKey = value;
                }
            }
        }

        /// <summary>
        /// Set the value of the property to a global auto-incrementing integer. This is shared accross all properties that use this method.
        /// </summary>
        /// <typeparam name="TPropertyConfiguration">The type of the property configuration.</typeparam>
        /// <param name="builder">The property configuration builder instance.</param>
        /// <returns>The property configuration builder instance.</returns>
        public static IPropertyConfigurationBuilder<int?, TPropertyConfiguration> AutoIncrementingInteger<TPropertyConfiguration>(
            this IComputedValueBuilder<int?, TPropertyConfiguration> builder)
           where TPropertyConfiguration : IInMemoryPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc.ValueComputationFunc = () => Interlocked.Increment(ref _globalPrimaryKey);

            // Any property that has a computed value is by definition not read-only, so explicitly enforce this
            var propertyBuilder = builder as IPropertyConfigurationBuilder<int?, TPropertyConfiguration>;
            propertyBuilder.IsReadOnly(false);

            return propertyBuilder;
        }

        /// <summary>
        /// Set the value of the property to a global auto-incrementing integer. This is shared accross all properties that use this method.
        /// </summary>
        /// <typeparam name="TPropertyConfiguration">The type of the property configuration.</typeparam>
        /// <param name="builder">The property configuration builder instance.</param>
        /// <returns>The property configuration builder instance.</returns>
        public static IPropertyConfigurationBuilder<int, TPropertyConfiguration> AutoIncrementingInteger<TPropertyConfiguration>(
            this IComputedValueBuilder<int, TPropertyConfiguration> builder)
           where TPropertyConfiguration : IInMemoryPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc.ValueComputationFunc = () => Interlocked.Increment(ref _globalPrimaryKey);

            // Any property that has a computed value is by definition not read-only, so explicitly enforce this
            var propertyBuilder = builder as IPropertyConfigurationBuilder<int, TPropertyConfiguration>;
            propertyBuilder.IsReadOnly(false);

            return propertyBuilder;
        }

        /// <summary>
        /// Set the value of the property to a randomly generated GUID.
        /// </summary>
        /// <typeparam name="TPropertyConfiguration">The type of the property configuration.</typeparam>
        /// <param name="builder">The property configuration builder instance.</param>
        /// <returns>The property configuration builder instance.</returns>
        public static IPropertyConfigurationBuilder<Guid?, TPropertyConfiguration> RandomlyGeneratedGuid<TPropertyConfiguration>(
            this IComputedValueBuilder<Guid?, TPropertyConfiguration> builder)
           where TPropertyConfiguration : IInMemoryPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc.ValueComputationFunc = () => Guid.NewGuid();

            // Any property that has a computed value is by definition not read-only, so explicitly enforce this
            var propertyBuilder = builder as IPropertyConfigurationBuilder<Guid?, TPropertyConfiguration>;
            propertyBuilder.IsReadOnly(false);

            return propertyBuilder;
        }

        /// <summary>
        /// Set the value of the property to a randomly generated GUID.
        /// </summary>
        /// <typeparam name="TPropertyConfiguration">The type of the property configuration.</typeparam>
        /// <param name="builder">The property configuration builder instance.</param>
        /// <returns>The property configuration builder instance.</returns>
        public static IPropertyConfigurationBuilder<Guid, TPropertyConfiguration> RandomlyGeneratedGuid<TPropertyConfiguration>(
            this IComputedValueBuilder<Guid, TPropertyConfiguration> builder)
           where TPropertyConfiguration : IInMemoryPropertyConfiguration
        {
            builder.PropertyConfiguration.ValueComputationFunc.ValueComputationFunc = () => Guid.NewGuid();

            // Any property that has a computed value is by definition not read-only, so explicitly enforce this
            var propertyBuilder = builder as IPropertyConfigurationBuilder<Guid, TPropertyConfiguration>;
            propertyBuilder.IsReadOnly(false);

            return propertyBuilder;
        }
    }
}
