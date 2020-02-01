using System;

namespace Snoozle.Abstractions
{
    public interface IRuntimeConfiguration
    {
        Func<object, object> GetPrimaryKeyValue { get; }

        HttpVerb AllowedVerbsFlags { get; }

        string Route { get; }
    }
}
