using System.Collections.Generic;

namespace Snoozle.Abstractions
{
    public interface IResourceConfigurationBuilder<TPropertyConfiguration, TResourceConfiguration, TModelConfiguration>
        where TPropertyConfiguration : class, IPropertyConfiguration
        where TResourceConfiguration : class, IResourceConfiguration<TPropertyConfiguration, TModelConfiguration>
        where TModelConfiguration : class, IModelConfiguration
    {
        TResourceConfiguration BuildResourceConfiguration();

        IEnumerable<TPropertyConfiguration> PropertyConfigurations { get; }

        TModelConfiguration ModelConfiguration { get; }
    }
}
