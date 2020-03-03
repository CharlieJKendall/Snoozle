using Snoozle.Abstractions;
using System;
using System.Collections.Generic;

namespace Snoozle.InMemory.Implementation
{
    internal class InMemoryRuntimeConfigurationProvider : BaseRuntimeConfigurationProvider<IInMemoryRuntimeConfiguration<IRestResource>>, IInMemoryRuntimeConfigurationProvider
    {
        public InMemoryRuntimeConfigurationProvider(Dictionary<Type, IInMemoryRuntimeConfiguration<IRestResource>> runtimeConfigurations)
            : base(runtimeConfigurations)
        {
        }
    }
}
