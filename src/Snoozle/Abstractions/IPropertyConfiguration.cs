using System;

namespace Snoozle.Abstractions
{
    public interface IPropertyConfiguration
    {
        string PropertyName { get; set; }

        Type PropertyType { get; set; }

        int Index { get; set; }

        bool IsReadOnly { get; set; }

        bool IsPrimaryResourceIdentifier { get; set; }

        Func<object> ValueComputationFunc { get; set; }
    }
}
