using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Snoozle.Abstractions;
using Snoozle.InMemory.Configuration;
using Snoozle.InMemory.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Snoozle.InMemory
{
    public static class ServiceCollectionExtensions
    {
        public static IMvcBuilder AddSnoozleInMemory(this IMvcBuilder @this, IConfigurationSection configurationSection)
        {
            IServiceCollection serviceCollection = @this.Services;
            IInMemoryRuntimeConfigurationProvider runtimeConfigurationProvider = BuildRuntimeConfigurationProvider();

            serviceCollection.Configure<InMemorySnoozleOptions>(options => configurationSection.Bind(options));

            AddSnoozleInMemory(serviceCollection, runtimeConfigurationProvider);

            @this.AddSnoozleCore(runtimeConfigurationProvider, configurationSection);

            return @this;
        }

        public static IMvcBuilder AddSnoozleInMemory(this IMvcBuilder @this, Action<InMemorySnoozleOptions> optionsBuilder)
        {
            IServiceCollection serviceCollection = @this.Services;
            IInMemoryRuntimeConfigurationProvider runtimeConfigurationProvider = BuildRuntimeConfigurationProvider();

            serviceCollection.Configure(optionsBuilder);

            AddSnoozleInMemory(serviceCollection, runtimeConfigurationProvider);

            @this.AddSnoozleCore(runtimeConfigurationProvider, optionsBuilder);

            return @this;
        }

        public static IServiceCollection AddSnoozleInMemory(this IServiceCollection @this, IInMemoryRuntimeConfigurationProvider runtimeConfigurationProvider)
        {
            @this.AddScoped<IDataProvider, InMemoryDataProvider>();
            @this.AddSingleton(runtimeConfigurationProvider);

            return @this;
        }

        private static IInMemoryRuntimeConfigurationProvider BuildRuntimeConfigurationProvider()
        {
            IEnumerable<IInMemoryResourceConfiguration> resourceConfigurations =
                ResourceConfigurationBuilder.Build<IInMemoryPropertyConfiguration, IInMemoryResourceConfiguration, IInMemoryModelConfiguration>(typeof(InMemoryResourceConfigurationBuilder<>));

            var runtimeConfigurations = new Dictionary<Type, IInMemoryRuntimeConfiguration<IRestResource>>();

            foreach (IInMemoryResourceConfiguration configuration in resourceConfigurations)
            {
                object resourceList = null;

                // Read any initial data from the JSON file specified if possible
                if (configuration.ModelConfiguration.JsonFilePath != null)
                {
                    string json = File.ReadAllText(configuration.ModelConfiguration.JsonFilePath);
                    Type genericListType = typeof(List<>).MakeGenericType(configuration.ResourceType);
                    MethodInfo genericMethod = typeof(JsonConvert)
                        .GetMethod(nameof(JsonConvert.DeserializeObject), 1, new Type[] { typeof(string) })
                        .MakeGenericMethod(genericListType);

                    // Create the generic list of resource objects
                    resourceList = genericMethod.Invoke(null, new object[] { json });
                }

                var runtimeConfiguration = Activator.CreateInstance(
                    typeof(InMemoryRuntimeConfiguration<>).MakeGenericType(configuration.ResourceType),
                    configuration,
                    resourceList) as IInMemoryRuntimeConfiguration<IRestResource>;

                runtimeConfigurations.Add(configuration.ResourceType, runtimeConfiguration);
            }

            return new InMemoryRuntimeConfigurationProvider(runtimeConfigurations);
        }
    }
}