using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Linq;

namespace Snoozle.Core
{
    internal class RestResourceControllerModelConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            string resourceName = controller.ControllerType.GetGenericArguments().SingleOrDefault(arg => arg.GetInterfaces().Contains(typeof(IRestResource)))?.Name;

            if (!string.IsNullOrEmpty(resourceName))
            {                
                controller.ControllerName = resourceName;
            }
        }
    }
}
