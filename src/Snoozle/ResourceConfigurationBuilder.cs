using Snoozle.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snoozle
{
    public static class ResourceConfigurationBuilder
    {
        /// <summary>
        /// Builds all the resource configurations for a given type.
        /// </summary>
        /// <typeparam name="TPropertyConfiguration">The interface derived from <see cref="IPropertyConfiguration"/>.</typeparam>
        /// <typeparam name="TResourceConfiguration">The interface derived from <see cref="IResourceConfiguration{TPropertyConfiguration, TModelConfiguration}"/>.</typeparam>
        /// <typeparam name="TModelConfiguration">The interface derived from <see cref="IModelConfiguration"/>.</typeparam>
        /// <param name="resourceConfigurationBuilderType">The type that implements <see cref="BaseResourceConfigurationBuilder{TResource, TPropertyConfiguration, TResourceConfiguration, TModelConfiguration}"/>.</param>
        /// <returns>Returns a single configuration for each rest resource defined.</returns>
        public static IEnumerable<TResourceConfiguration> Build<TPropertyConfiguration, TResourceConfiguration, TModelConfiguration>(Type resourceConfigurationBuilderType)
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
