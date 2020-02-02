using Snoozle.Enums;

namespace Snoozle.Abstractions
{
    public abstract class BaseModelConfiguration<TResource> : IModelConfiguration
        where TResource : class, IRestResource
    {
        public string Route { get; set; } = typeof(TResource).Name + "s";

        public HttpVerb AllowedVerbsFlags { get; set; } = HttpVerb.All;
    }
}
