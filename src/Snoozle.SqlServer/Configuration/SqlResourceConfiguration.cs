using Snoozle.Abstractions;
using System.Collections.Generic;

namespace Snoozle.SqlServer.Configuration
{
    public class SqlResourceConfiguration<TResource> : BaseResourceConfiguration<TResource, ISqlPropertyConfiguration, ISqlModelConfiguration>, ISqlResourceConfiguration
        where TResource : class, IRestResource
    {
        public SqlResourceConfiguration(
            ISqlModelConfiguration modelConfiguration,
            IEnumerable<ISqlPropertyConfiguration> propertyConfigurations)
            : base(modelConfiguration, propertyConfigurations)
        {
        }
    }
}
