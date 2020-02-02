using Snoozle.Enums;

namespace Snoozle.Abstractions
{
    public interface IModelConfigurationBuilder<TModelConfiguration>
        where TModelConfiguration : class, IModelConfiguration
    {
        TModelConfiguration ModelConfiguration { get; }

        IModelConfigurationBuilder<TModelConfiguration> HasAllowedHttpVerbs(HttpVerb allowedVerbsFlags);

        IModelConfigurationBuilder<TModelConfiguration> HasRoute(string route);
    }
}