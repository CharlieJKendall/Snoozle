using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Snoozle.Configuration;
using System;
using System.Linq;

namespace Snoozle.Core
{
    internal class RestResourceControllerModelConvention : IControllerModelConvention
    {
        private readonly IRuntimeConfigurationProvider _runtimeConfigurationProvider;

        public RestResourceControllerModelConvention(IRuntimeConfigurationProvider runtimeConfigurationProvider)
        {
            _runtimeConfigurationProvider = runtimeConfigurationProvider;
        }

        public void Apply(ControllerModel controller)
        {
            var resourceType = controller.ControllerType.GetGenericArguments().SingleOrDefault(arg => arg.GetInterfaces().Contains(typeof(IRestResource)));

            if (resourceType != null)
            {
                var config = _runtimeConfigurationProvider.GetRuntimeConfigurationForType(resourceType);
                controller.ControllerName = config.Route;
            }
        }
    }
}
