using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snoozle.Core
{
    internal class RestResourceControllerModelConvention : IControllerModelConvention
    {
        private readonly Dictionary<Type, string> _routes;

        internal RestResourceControllerModelConvention(Dictionary<Type, string> routes)
        {
            _routes = routes;
        }

        public void Apply(ControllerModel controller)
        {
            Type resourceType = controller.ControllerType.GetGenericArguments().SingleOrDefault(arg => arg.GetInterfaces().Contains(typeof(IRestResource)));

            if (resourceType != null)
            {
                controller.ControllerName = _routes.GetValueOrDefault(resourceType)
                    ?? throw new InvalidOperationException($"No route found for resource {resourceType.Name}.");
            }
        }
    }
}
