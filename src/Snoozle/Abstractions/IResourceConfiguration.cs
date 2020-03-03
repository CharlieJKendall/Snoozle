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

        HttpVerbs AllowedVerbsFlags { get; }

        IEnumerable<TPropertyConfiguration> PropertyConfigurationsForRead { get; }

        TPropertyConfiguration PrimaryIdentifier { get; }

        IEnumerable<TPropertyConfiguration> PropertyConfigurationsForCreate { get; }

        IEnumerable<TPropertyConfiguration> PropertyConfigurationsForUpdate { get; }

        string Route { get; }

        Type ResourceType { get; }
    }
}
