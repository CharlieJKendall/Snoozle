using System;

namespace Snoozle.Extensions
{
    public static class ExceptionHelper
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

        public static class Argument
        {
            public static void ThrowIfTrue(bool @bool, string message, string paramName)
            {
                if (@bool)
                {
                    throw new ArgumentException(message, paramName);
                }
            }
        }

        public static class InvalidOperation
        {
            public static void ThrowIfTrue(bool @bool, string message)
            {
                if (@bool)
                {
                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}
