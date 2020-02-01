using Snoozle.SqlServer.Interfaces;
using System;

namespace Snoozle.SqlServer
{
    public class SqlParameterProvider : ISqlParamaterProvider
    {
        private const string ID_PARAM_NAME = "PrimaryKeyIdParam";

        public string GenerateParameterName(string propertyName)
        {
            if (propertyName == ID_PARAM_NAME)
            {
                throw new ArgumentException($"Property cannot be called '{ID_PARAM_NAME}'; this is reserved for internal usage.", nameof(propertyName));
            }

            return $"@{propertyName}";
        }

        public string GetPrimaryKeyParameterName()
        {
            return $"@{ID_PARAM_NAME}";
        }
    }
}
