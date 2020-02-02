using System;

namespace Snoozle
{
    public static class ExceptionHelper
    {
        public static class ArgumentNull
        {
            /// <summary>
            /// Throws a <see cref="ArgumentNullException"/> if the parameter is null.
            /// </summary>
            /// <param name="obj">The parameter to test.</param>
            /// <param name="paramName">The name of the parameter.</param>
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
            /// <summary>
            /// Throws a <see cref="ArgumentException"/> if a condition is true.
            /// </summary>
            /// <param name="bool">The condition to evaluate.</param>
            /// <param name="message">The exception message.</param>
            /// <param name="paramName">The name of the parameter.</param>
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
            /// <summary>
            /// Throws a <see cref="InvalidOperationException"/> if a condition is true.
            /// </summary>
            /// <param name="bool">The condition to evaluate.</param>
            /// <param name="message">The exception message.</param>
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
