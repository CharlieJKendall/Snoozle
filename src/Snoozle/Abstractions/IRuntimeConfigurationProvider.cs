using System;

namespace Snoozle.Abstractions
{
    public interface IRuntimeConfigurationProvider<out TRuntimeConfiguration>
        where TRuntimeConfiguration : IRuntimeConfiguration
    {
        TRuntimeConfiguration GetRuntimeConfigurationForType(Type type);
    }
}
