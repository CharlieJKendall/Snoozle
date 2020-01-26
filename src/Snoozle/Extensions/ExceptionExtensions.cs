using System;

namespace Snoozle.Extensions
{
    public static class ExceptionExtensions
    {
        public static class ArgumentNull
        {
            public static void ThrowIfNecessary(object obj, string paramName)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException(paramName);
                }
            }
        }
    }
}
