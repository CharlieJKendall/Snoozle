namespace Snoozle.Abstractions
{
    /// <summary>
    /// Provides core configuration options that can be configured at the model level.
    /// </summary>
    /// <typeparam name="TResource">The resource type that this configuration applies to.</typeparam>
    public abstract class BaseModelConfiguration<TResource> : IModelConfiguration
        where TResource : class, IRestResource
    {
        /// <summary>
        /// The API route that model data can be accessed at.
        /// </summary>
        /// <remarks>Defaults to the name of the resource type with the letter "s" appended.</remarks>
        public string Route { get; set; } = typeof(TResource).Name + "s";

        /// <summary>
        /// The HTTP verbs that can be used when accessing the specified resource.
        /// </summary>
        /// <remarks>Defaults to <see cref="HttpVerb.All"/>.</remarks>
        public HttpVerb AllowedVerbsFlags { get; set; } = HttpVerb.All;
    }
}
