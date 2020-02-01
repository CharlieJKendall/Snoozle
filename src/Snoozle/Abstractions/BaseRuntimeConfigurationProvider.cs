using System;
using System.Collections.Generic;

namespace Snoozle.Abstractions
{
    public abstract class BaseRuntimeConfigurationProvider<TRuntimeConfiguration> : IRuntimeConfigurationProvider<TRuntimeConfiguration>
        where TRuntimeConfiguration : class, IRuntimeConfiguration
    {
        private readonly Dictionary<Type, TRuntimeConfiguration> _runtimeConfigurations;

        protected BaseRuntimeConfigurationProvider(Dictionary<Type, TRuntimeConfiguration> runtimeConfigurations)
        {
            _runtimeConfigurations = runtimeConfigurations;
        }

        public TRuntimeConfiguration GetRuntimeConfigurationForType(Type type)
        {
            return _runtimeConfigurations.GetValueOrDefault(type);
        }
    }
}
