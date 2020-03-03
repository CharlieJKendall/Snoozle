using Snoozle.Abstractions;

namespace Snoozle.InMemory.Implementation
{
    public class InMemoryModelConfigurationBuilder : BaseModelConfigurationBuilder<IInMemoryModelConfiguration>, IModelConfigurationBuilder<IInMemoryModelConfiguration>
    {
        public InMemoryModelConfigurationBuilder(IInMemoryModelConfiguration modelConfiguration)
            : base(modelConfiguration)
        {
        }
    }
}