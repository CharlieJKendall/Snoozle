using Snoozle.Abstractions.Models;
using System;
using System.Linq.Expressions;

namespace Snoozle.Abstractions
{
    public interface IPropertyConfiguration
    {
        string PropertyName { get; set; }

        Type PropertyType { get; set; }

        int Index { get; set; }

        bool IsReadOnly { get; set; }

        bool IsPrimaryResourceIdentifier { get; set; }

        ValueComputationFuncModel ValueComputationFunc { get; set; }

        bool HasComputedValue { get; }

        bool HasComputationEndpointTrigger(HttpVerbs endpoint);
    }
}
