using Snoozle.Abstractions;
using System;
using System.Collections.Generic;

namespace Snoozle.SqlServer.Configuration
{
    public class SqlRuntimeConfigurationProvider : BaseRuntimeConfigurationProvider<ISqlRuntimeConfiguration<IRestResource>>, ISqlRuntimeConfigurationProvider
    {
        public SqlRuntimeConfigurationProvider(Dictionary<Type, ISqlRuntimeConfiguration<IRestResource>> runtimeConfigurations)
            : base(runtimeConfigurations)
        {
        }
    }
}
