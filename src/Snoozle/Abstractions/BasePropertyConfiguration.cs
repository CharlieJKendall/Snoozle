using Snoozle.Abstractions.Models;
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

        public ValueComputationFuncModel ValueComputationFunc { get; set; } = null;
    }
}
