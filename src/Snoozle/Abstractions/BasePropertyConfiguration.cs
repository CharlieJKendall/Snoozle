using System;

namespace Snoozle.Abstractions
{
    public abstract class BasePropertyConfiguration : IPropertyConfiguration
    {
        public string PropertyName { get; set; }

        public Type PropertyType { get; set; }

        public int Index { get; set;  }

        public bool IsReadOnly { get; set; } = false;

        public bool IsPrimaryResourceIdentifier { get; set; } = false;

        public Func<object> ValueComputationFunc { get; set; } = null;
    }
}
