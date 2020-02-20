using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Snoozle.Abstractions
{
    /// <summary>
    /// Provides functionality to retrieve any given runtime configuration.
    /// </summary>
    public abstract class BaseRuntimeConfigurationProvider<TRuntimeConfiguration> : IRuntimeConfigurationProvider<TRuntimeConfiguration>
        where TRuntimeConfiguration : class, IRuntimeConfiguration
    {
        private readonly ReadOnlyDictionary<Type, TRuntimeConfiguration> _runtimeConfigurations;

        /// <summary>
        /// Initialises a new instance of the <see cref="BaseRuntimeConfigurationProvider{TRuntimeConfiguration}"/> class.
        /// </summary>
        /// <param name="runtimeConfigurations"></param>
        protected BaseRuntimeConfigurationProvider(Dictionary<Type, TRuntimeConfiguration> runtimeConfigurations)
        {
            _runtimeConfigurations = new ReadOnlyDictionary<Type, TRuntimeConfiguration>(runtimeConfigurations);
        }

        /// <summary>
        /// The REST resource types that are configured.
        /// </summary>
        public List<Type> TypesConfigured => _runtimeConfigurations.Keys.ToList();

        /// <summary>
        /// Gets the <see cref="IRuntimeConfiguration"/> instance for the specified REST resource type.
        /// </summary>
        /// <param name="type">The REST resource Type.</param>
        public TRuntimeConfiguration GetRuntimeConfigurationForType(Type type)
        {
            return _runtimeConfigurations.GetValueOrDefault(type);
        }
    }
}
