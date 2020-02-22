using Snoozle.Abstractions.Models;
using System;
using System.Collections.Generic;

namespace Snoozle.Abstractions
{
    public interface IRuntimeConfiguration
    {
        Func<object, object> GetPrimaryKeyValue { get; }

        HttpVerbs AllowedVerbsFlags { get; }

        string Route { get; }

        IEnumerable<ValueComputationActionModel> ValueComputationActions { get; }
    }
}
