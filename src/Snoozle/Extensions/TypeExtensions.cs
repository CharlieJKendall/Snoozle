using System;

namespace Snoozle.Extensions
{
    public static class TypeExtensions
    {
        public static bool TryUnwrapNullableType(this Type type, out Type unwrappedType)
        {
            bool wasUnwrapped = true;

            unwrappedType = Nullable.GetUnderlyingType(type);

            if (unwrappedType == null)
            {
                wasUnwrapped = false;
                unwrappedType = type;
            }

            return wasUnwrapped;
        }
    }
}
