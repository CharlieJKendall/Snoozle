using Snoozle.Enums;
using System;
using System.Collections.Generic;

namespace Snoozle.Abstractions
{
    public interface IResourceConfiguration<TPropertyConfiguration, TModelConfiguration>
        where TPropertyConfiguration : class, IPropertyConfiguration
        where TModelConfiguration : class, IModelConfiguration
    {
        TModelConfiguration ModelConfiguration { get; }

        IEnumerable<TPropertyConfiguration> AllPropertyConfigurations { get; }

        HttpVerb AllowedVerbsFlags { get; }

        IEnumerable<TPropertyConfiguration> PropertyConfigurationsForRead { get; }

        TPropertyConfiguration PrimaryIdentifier { get; }

        IEnumerable<TPropertyConfiguration> PropertyConfigurationsForWrite { get; }

        string Route { get; }

        Type ResourceType { get; }
    }
}
