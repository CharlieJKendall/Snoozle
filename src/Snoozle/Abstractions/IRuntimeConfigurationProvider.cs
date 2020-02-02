using System;
using System.Collections.Generic;

namespace Snoozle.Abstractions
{
    public interface IRuntimeConfigurationProvider<out TRuntimeConfiguration>
        where TRuntimeConfiguration : IRuntimeConfiguration
    {
        List<Type> TypesConfigured { get; }
        TRuntimeConfiguration GetRuntimeConfigurationForType(Type type);
    }
}
