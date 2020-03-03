using Snoozle.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snoozle.InMemory.Implementation
{
    internal class InMemoryDataProvider : IDataProvider
    {
        private readonly IInMemoryRuntimeConfigurationProvider _inMemoryRuntimeConfigurationProvider;

        public InMemoryDataProvider(IInMemoryRuntimeConfigurationProvider inMemoryRuntimeConfigurationProvider)
        {
            _inMemoryRuntimeConfigurationProvider = inMemoryRuntimeConfigurationProvider;
        }

        public Task<bool> DeleteByIdAsync<TResource>(object primaryKey)
            where TResource : class, IRestResource
        {
            IInMemoryRuntimeConfiguration<TResource> config = GetConfig<TResource>();

            return Task.FromResult(config.DeleteEntry(primaryKey.ToString()));
        }

        public Task<TResource> InsertAsync<TResource>(TResource resourceToCreate)
            where TResource : class, IRestResource
        {
            IInMemoryRuntimeConfiguration<TResource> config = GetConfig<TResource>();

            return Task.FromResult(config.InsertEntry(resourceToCreate));
        }

        public Task<IEnumerable<TResource>> SelectAllAsync<TResource>()
            where TResource : class, IRestResource
        {
            IInMemoryRuntimeConfiguration<TResource> config = GetConfig<TResource>();

            return Task.FromResult(config.GetAllEntries());
        }

        public Task<TResource> SelectByIdAsync<TResource>(object primaryKey)
            where TResource : class, IRestResource
        {
            IInMemoryRuntimeConfiguration<TResource> config = GetConfig<TResource>();

            return Task.FromResult(config.GetEntryByPrimaryKey(primaryKey.ToString()));
        }

        public Task<TResource> UpdateAsync<TResource>(TResource resourceToUpdate, object primaryKey)
            where TResource : class, IRestResource
        {
            IInMemoryRuntimeConfiguration<TResource> config = GetConfig<TResource>();

            return Task.FromResult(config.UpdateEntry(resourceToUpdate, primaryKey.ToString()));
        }

        private IInMemoryRuntimeConfiguration<TResource> GetConfig<TResource>()
            where TResource : class, IRestResource
        {
            return (IInMemoryRuntimeConfiguration<TResource>)_inMemoryRuntimeConfigurationProvider.GetRuntimeConfigurationForType(typeof(TResource));
        }
    }
}
