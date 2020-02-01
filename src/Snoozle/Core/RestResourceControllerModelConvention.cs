using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Snoozle.Abstractions;
using System;
using System.Linq;

namespace Snoozle.Core
{
    internal class RestResourceControllerModelConvention : IControllerModelConvention
    {
        private readonly IRuntimeConfigurationProvider<IRuntimeConfiguration> _runtimeConfigurationProvider;

        public RestResourceControllerModelConvention(IRuntimeConfigurationProvider<IRuntimeConfiguration> runtimeConfigurationProvider)
        {
            _runtimeConfigurationProvider = runtimeConfigurationProvider;
        }

        public void Apply(ControllerModel controller)
        {
            var resourceType = controller.ControllerType.GetGenericArguments().SingleOrDefault(arg => arg.GetInterfaces().Contains(typeof(IRestResource)));

            if (resourceType != null)
            {
                IRuntimeConfiguration config = _runtimeConfigurationProvider.GetRuntimeConfigurationForType(resourceType);
                controller.ControllerName = config.Route;
            }
        }
    }
}
