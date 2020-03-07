using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Snoozle.Abstractions;
using Snoozle.Configuration;
using Snoozle.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snoozle
{
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds the core Snoozle functionality and configuration to the application. This should only be called from the data providers <see cref="IMvcBuilder"/> extensions,
        /// and not directly from the web application.
        /// </summary>
        public static IMvcBuilder AddSnoozleCore<TRuntimeConfiguration, TOptions>(
            this IMvcBuilder @this,
            IRuntimeConfigurationProvider<TRuntimeConfiguration> runtimeConfigurationProvider,
            Action<TOptions> optionsBuilder)
            where TRuntimeConfiguration : class, IRuntimeConfiguration
            where TOptions : SnoozleOptions
        {
            IServiceCollection serviceCollection = @this.Services;
            AddSnoozleCore(@this, runtimeConfigurationProvider);

            serviceCollection.Configure(optionsBuilder);

            return @this;
        }

        /// <summary>
        /// Adds the core Snoozle functionality and configuration to the application. This should only be called from the data providers <see cref="IMvcBuilder"/> extensions,
        /// and not directly from the web application.
        /// </summary>
        public static IMvcBuilder AddSnoozleCore<TRuntimeConfiguration>(
            this IMvcBuilder @this,
            IRuntimeConfigurationProvider<TRuntimeConfiguration> runtimeConfigurationProvider,
            IConfigurationSection configOptions)
            where TRuntimeConfiguration : class, IRuntimeConfiguration
        {
            IServiceCollection serviceCollection = @this.Services;
            AddSnoozleCore(@this, runtimeConfigurationProvider);

            serviceCollection.Configure<SnoozleOptions>(options => configOptions.Bind(options));

            return @this;
        }

        /// <summary>
        /// Adds the core Snoozle functionality and configuration to the application. This should only be called from the data providers <see cref="IMvcBuilder"/> extensions,
        /// and not directly from the web application.
        /// </summary>
        private static IMvcBuilder AddSnoozleCore<TRuntimeConfiguration>(
            this IMvcBuilder @this,
            IRuntimeConfigurationProvider<TRuntimeConfiguration> runtimeConfigurationProvider)
            where TRuntimeConfiguration : class, IRuntimeConfiguration
        {
            IServiceCollection serviceCollection = @this.Services;
            var baseRuntimeConfgurationProvider = runtimeConfigurationProvider as IRuntimeConfigurationProvider<IRuntimeConfiguration>;

            serviceCollection.AddSingleton(baseRuntimeConfgurationProvider);

            // Add controller types to a custom application part so they can be discovered correctly
            @this.ConfigureApplicationPartManager(manager => manager.ApplicationParts.Add(new RestResourceControllerApplicationPart(GetRestResourceControllerTypeInfos())));

            // Add custom controller model convention to ensure controller route matches resource name
            @this.AddMvcOptions(options => options.Conventions.Add(new RestResourceControllerModelConvention(GetCustomRoutes(baseRuntimeConfgurationProvider))));

            return @this;
        }

        private static Dictionary<Type, string> GetCustomRoutes(IRuntimeConfigurationProvider<IRuntimeConfiguration> baseRuntimeConfgurationProvider)
        {
            // Create a map of custom routes defined for the rest resources 
            return new Dictionary<Type, string>(
                baseRuntimeConfgurationProvider.TypesConfigured.Select(c => KeyValuePair.Create(c, baseRuntimeConfgurationProvider.GetRuntimeConfigurationForType(c).Route)));
        }

        private static IEnumerable<TypeInfo> GetRestResourceControllerTypeInfos()
        {
            // Get all rest resource implementations defined in application domain
            IEnumerable<TypeInfo> restResources = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .Where(restResource => restResource.ImplementedInterfaces.Contains(typeof(IRestResource)) && restResource.IsClass && !restResource.IsAbstract);

            // Create closed generic controller TypeInfo for each resource defined
            return restResources.Select(resource => typeof(RestResourceController<>).MakeGenericType(resource).GetTypeInfo());
        }
    }
}
