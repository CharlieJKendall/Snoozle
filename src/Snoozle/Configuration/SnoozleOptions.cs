using Snoozle.Enums;

namespace Snoozle.Configuration
{
    public class SnoozleOptions
    {
        public HttpVerb AllowedVerbs { get; set; } = HttpVerb.All;
    }
}
