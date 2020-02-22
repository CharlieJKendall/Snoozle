using Snoozle.Abstractions.Models;
using System;

namespace Snoozle.Abstractions
{
    /// <summary>
    /// Provides core configuration options that can be configured at the property level.
    /// </summary>
    public abstract class BasePropertyConfiguration : IPropertyConfiguration
    {
        /// <summary>
        /// The name of the property member.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The type of the property member.
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// The index of the property member.
        /// </summary>
        /// <remarks>
        /// This is used by data providers to enforce a predictable ordering of properties.
        /// </remarks>
        public int Index { get; set; }

        /// <summary>
        /// Whether the property is a read-only value.
        /// </summary>
        public bool IsReadOnly { get; set; } = false;

        /// <summary>
        /// Whether the property is the primary key/identifier for the REST resource.
        /// </summary>
        public bool IsPrimaryResourceIdentifier { get; set; } = false;

        /// <summary>
        /// A Func to be applied to the property during write operations.
        /// </summary>
        public ValueComputationFuncModel ValueComputationFunc { get; set; } = null;
    }
}
