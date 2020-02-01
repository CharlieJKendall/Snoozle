using Snoozle.Abstractions;

namespace Snoozle.SqlServer.Configuration
{
    public class SqlPropertyConfigurationBuilder<TResource, TProperty> : BasePropertyConfigurationBuilder<TResource, TProperty, ISqlPropertyConfiguration>, IPropertyConfigurationBuilder<TProperty, ISqlPropertyConfiguration>
        where TResource : class, IRestResource
    {
        public SqlPropertyConfigurationBuilder(ISqlPropertyConfiguration propertyConfiguration)
            : base(propertyConfiguration)
        {
        }
    }
}
