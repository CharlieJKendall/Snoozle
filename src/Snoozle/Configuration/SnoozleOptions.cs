namespace Snoozle.Configuration
{
    public class SnoozleOptions
    {
        /// <summary>
        /// Global HTTP method verbs allowed for the entire application. Can be overridden at the REST resource level via model configuration.
        /// </summary>
        public HttpVerb AllowedVerbs { get; set; } = HttpVerb.All;
    }
}
