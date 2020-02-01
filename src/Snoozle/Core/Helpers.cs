using Snoozle.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snoozle.Core
{
    public static class Helpers
    {
        public static IEnumerable<TResourceConfiguration> BuildResourceConfigurations<TPropertyConfiguration, TResourceConfiguration, TModelConfiguration>(Type resourceConfigurationBuilderType)
            where TPropertyConfiguration : class, IPropertyConfiguration
            where TResourceConfiguration : class, IResourceConfiguration<TPropertyConfiguration, TModelConfiguration>
            where TModelConfiguration : class, IModelConfiguration
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .Where(restResource => restResource.BaseType != null && restResource.BaseType.IsGenericType && restResource.BaseType.GetGenericTypeDefinition().IsAssignableFrom(resourceConfigurationBuilderType) && !restResource.IsAbstract)
                .Select(type => ((IResourceConfigurationBuilder<TPropertyConfiguration, TResourceConfiguration, TModelConfiguration>)Activator.CreateInstance(type)).BuildResourceConfiguration());
        }
    }
}
