using System;
using System.Collections.Generic;

namespace Snoozle.Configuration
{
    public class StaticRuntimeConfigurationProvider : IRuntimeConfigurationProvider
    {
        private static Dictionary<Type, IRuntimeConfiguration> _runtimeConfigurations = new Dictionary<Type, IRuntimeConfiguration>();

        public void AddRuntimeConfigurationForType(Type type, IRuntimeConfiguration configuration)
        {
            _runtimeConfigurations.Add(type, configuration);
        }

        public IRuntimeConfiguration GetRuntimeConfigurationForType(Type type)
        {
            return _runtimeConfigurations.GetValueOrDefault(type);
        }
    }

    public interface IRuntimeConfigurationProvider
    {
        IRuntimeConfiguration GetRuntimeConfigurationForType(Type type);
        void AddRuntimeConfigurationForType(Type type, IRuntimeConfiguration configuration);
    }
}
