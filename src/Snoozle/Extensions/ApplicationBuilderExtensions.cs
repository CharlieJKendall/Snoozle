using Microsoft.AspNetCore.Builder;

namespace Snoozle.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSnoozle(this IApplicationBuilder @this)
        {
            return @this;
        }
    }
}
