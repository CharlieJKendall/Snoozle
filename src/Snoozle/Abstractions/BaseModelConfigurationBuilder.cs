using Snoozle.Extensions;
using System.Text.RegularExpressions;

namespace Snoozle.Abstractions
{
    public abstract class BaseModelConfigurationBuilder<TModelConfiguration> : IModelConfigurationBuilder<TModelConfiguration>
        where TModelConfiguration : class, IModelConfiguration
    {
        private readonly Regex _routeRegex = new Regex(@"^[A-Za-z0-9]+$");

        protected BaseModelConfigurationBuilder(TModelConfiguration modelConfiguration)
        {
            ModelConfiguration = modelConfiguration;
        }

        public TModelConfiguration ModelConfiguration { get; }

        public IModelConfigurationBuilder<TModelConfiguration> HasAllowedHttpVerbs(HttpVerb allowedVerbsFlags)
        {
            ModelConfiguration.AllowedVerbsFlags = allowedVerbsFlags;
            return this;
        }

        public IModelConfigurationBuilder<TModelConfiguration> HasRoute(string route)
        {
            ExceptionHelper.ArgumentNull.ThrowIfNecessary(route, nameof(route));
            ExceptionHelper.Argument.ThrowIfTrue(!_routeRegex.IsMatch(route), $"'{route}' is an invalid route.", nameof(route));

            ModelConfiguration.Route = route;

            return this;
        }
    }
}
