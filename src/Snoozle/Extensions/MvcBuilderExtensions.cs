using Microsoft.Extensions.DependencyInjection;
using Snoozle.Configuration;
using Snoozle.Core;
using Snoozle.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Snoozle.Extensions
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddSnoozle(this IMvcBuilder @this)
        {
            IServiceCollection serviceCollection = @this.Services;

            serviceCollection.AddScoped<ISqlExecutor, SqlExecutor>();

            IRuntimeConfigurationProvider runtimeConfig = RuntimeConfigurationProvider.CreateInstance();
            serviceCollection.AddSingleton<IRuntimeConfigurationProvider>(runtimeConfig);

            // Get all rest resources defined in application domain
            IEnumerable<TypeInfo> restResources = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .Where(restResource => restResource.ImplementedInterfaces.Contains(typeof(IRestResource)) && restResource.IsClass && !restResource.IsAbstract);

            // Create closed generic controller TypeInfo for each resource defined
            IEnumerable<TypeInfo> controllerTypeInfos = restResources.Select(resource => typeof(RestResourceController<>).MakeGenericType(resource).GetTypeInfo());

            // Add controller types to a custom application part so they can be discovered correctly
            @this.ConfigureApplicationPartManager(manager => manager.ApplicationParts.Add(new RestResourceControllerApplicationPart(controllerTypeInfos)));

            // Add custom controller model convention to ensure controller route matches resource name
            @this.AddMvcOptions(options => options.Conventions.Add(new RestResourceControllerModelConvention(runtimeConfig)));            

            return @this;
        }
    }
}
