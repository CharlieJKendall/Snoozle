using System;

namespace Snoozle.SqlServer.Extensions
{
    public static class TypeExtensions
    {
        public static bool TryUnwrapNullableType(this Type type, out Type unwrappedType)
        {
            unwrappedType = Nullable.GetUnderlyingType(type) ?? type;

            return unwrappedType != type;
        }
    }
}
