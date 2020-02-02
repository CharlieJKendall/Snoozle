using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Snoozle.Abstractions
{
    public abstract class BaseRuntimeConfigurationProvider<TRuntimeConfiguration> : IRuntimeConfigurationProvider<TRuntimeConfiguration>
        where TRuntimeConfiguration : class, IRuntimeConfiguration
    {
        private readonly ReadOnlyDictionary<Type, TRuntimeConfiguration> _runtimeConfigurations;

        protected BaseRuntimeConfigurationProvider(Dictionary<Type, TRuntimeConfiguration> runtimeConfigurations)
        {
            _runtimeConfigurations = new ReadOnlyDictionary<Type, TRuntimeConfiguration>(runtimeConfigurations);
        }

        public List<Type> TypesConfigured => _runtimeConfigurations.Keys.ToList();

        public TRuntimeConfiguration GetRuntimeConfigurationForType(Type type)
        {
            return _runtimeConfigurations.GetValueOrDefault(type);
        }
    }
}
