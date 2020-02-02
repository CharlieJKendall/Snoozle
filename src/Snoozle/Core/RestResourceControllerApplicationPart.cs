using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Collections.Generic;
using System.Reflection;

namespace Snoozle.Core
{
    internal class RestResourceControllerApplicationPart : ApplicationPart, IApplicationPartTypeProvider
    {
        public RestResourceControllerApplicationPart(IEnumerable<TypeInfo> restResourceTypeInfos)
        {
            Types = restResourceTypeInfos;
        }

        public override string Name => typeof(RestResourceController<>).Name;

        public IEnumerable<TypeInfo> Types { get; }
    }
}
