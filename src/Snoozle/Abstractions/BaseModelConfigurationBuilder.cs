using Snoozle.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace Snoozle.Abstractions
{
    /// <summary>
    /// Provides methods for setting the core model configuration values.
    /// </summary>
    public abstract class BaseModelConfigurationBuilder<TModelConfiguration> : IModelConfigurationBuilder<TModelConfiguration>
        where TModelConfiguration : class, IModelConfiguration
    {
        /// <summary>
        /// Regex to guard againt unsafe custom route definitons.
        /// </summary>
        private readonly Regex _routeRegex = new Regex(@"^[A-Za-z0-9]+$");

        /// <summary>
        /// Initialises a the <see cref="BaseModelConfigurationBuilder{TModelConfiguration}"/> class.
        /// </summary>
        /// <param name="modelConfiguration">The model configuration that the inheriting builder constructs.</param>
        protected BaseModelConfigurationBuilder(TModelConfiguration modelConfiguration)
        {
            ModelConfiguration = modelConfiguration;
        }

        /// <summary>
        /// The model configuration.
        /// </summary>
        public TModelConfiguration ModelConfiguration { get; }

        /// <summary>
        /// Sets the HTTP verbs that can be used to access this model.
        /// </summary>
        /// <param name="allowedVerbsFlags">The allowed verbs for this model, i.e HttpVerb.GET | HttpVerb.POST</param>
        /// <remarks><see cref="HttpVerbs"/> is decorated with the <see cref="FlagsAttribute"/> attribute.</remarks>
        public IModelConfigurationBuilder<TModelConfiguration> HasAllowedHttpVerbs(HttpVerbs allowedVerbsFlags)
        {
            ModelConfiguration.AllowedVerbsFlags = allowedVerbsFlags;
            return this;
        }

        /// <summary>
        /// Sets the API route that this model can be accessed at.
        /// </summary>
        /// <param name="route">The route to use.</param>
        public IModelConfigurationBuilder<TModelConfiguration> HasRoute(string route)
        {
            ExceptionHelper.ArgumentNull.ThrowIfNecessary(route, nameof(route));
            ExceptionHelper.Argument.ThrowIfTrue(!_routeRegex.IsMatch(route), $"'{route}' is an invalid route.", nameof(route));

            ModelConfiguration.Route = route;

            return this;
        }
    }
}
