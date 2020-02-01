using Snoozle.Abstractions;

namespace Snoozle.SqlServer.Configuration
{
    public class SqlModelConfigurationBuilder : BaseModelConfigurationBuilder<ISqlModelConfiguration>, IModelConfigurationBuilder<ISqlModelConfiguration>
    {
        public SqlModelConfigurationBuilder(ISqlModelConfiguration modelConfiguration)
            : base(modelConfiguration)
        {
        }
    }
}