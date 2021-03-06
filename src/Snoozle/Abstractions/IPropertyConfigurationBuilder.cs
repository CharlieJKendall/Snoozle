﻿namespace Snoozle.Abstractions
{
    public interface IPropertyConfigurationBuilderCore<TProperty, TPropertyConfiguration>
    {
        /// <summary>
        /// The property configuration this builder is for.
        /// </summary>
        TPropertyConfiguration PropertyConfiguration { get; }
    }

    public interface IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> : IPropertyConfigurationBuilderCore<TProperty, TPropertyConfiguration>
    {
        /// <summary>
        /// Sets the property to read-only.
        /// </summary>
        IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> IsReadOnly(bool isReadOnly = true);

        /// <summary>
        /// Sets the property as the primary key/identifier for the resource. Only one primary identifier can be set per resource.
        /// </summary>
        /// <param name="isValueAutoGeneratedByDataStore">
        /// True if the primary identifier value is determined automatically by the data persistence layer (e.g. auto-incrementing PK).
        /// </param>
        IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> IsPrimaryIdentifier(bool isValueAutoGeneratedByDataStore = true);

        /// <summary>
        /// Gets a <see cref="IComputedValueBuilder{TProperty, TPropertyConfiguration}"/> that sets the value of the property automatically on write operations.
        /// </summary>
        /// <param name="endpointTriggers">The endpoints that will trigger this computed value. Values can be one or more of:<see cref="HttpVerbs.PUT"/>, <see cref="HttpVerbs.POST"/></param>
        IComputedValueBuilder<TProperty, TPropertyConfiguration> HasComputedValue(HttpVerbs endpointTriggers);
    }

    public interface IComputedValueBuilder<TProperty, TPropertyConfiguration> : IPropertyConfigurationBuilderCore<TProperty, TPropertyConfiguration>
    {
    }
}
