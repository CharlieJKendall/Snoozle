using Snoozle.Abstractions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Snoozle.InMemory.Implementation
{
    internal class InMemoryRuntimeConfiguration<TResource> : BaseRuntimeConfiguration<IInMemoryPropertyConfiguration, IInMemoryModelConfiguration, TResource>, IInMemoryRuntimeConfiguration<TResource>
        where TResource : class, IRestResource
    {
        private readonly ConcurrentDictionary<string, TResource> _data;

        // This is created by reflection, so be careful when changing/adding parameters
        public InMemoryRuntimeConfiguration(IInMemoryResourceConfiguration resourceConfiguration, List<TResource> initialData)
            : base(resourceConfiguration)
        {
            initialData = initialData ?? new List<TResource>();
            _data = new ConcurrentDictionary<string, TResource>(initialData?.ToDictionary(x => GetPrimaryKeyValue(x).ToString()));
            PropertyConfigurationBuilderExtensions.SetGlobalPrimaryKeyInteger(GetMaxPrimaryKeyIntegerValueOrDefault());
        }

        private int GetMaxPrimaryKeyIntegerValueOrDefault()
        {
            try
            {
                return _data.Keys.Select(x => int.Parse(x)).Max();
            }
            catch
            {
                return default;
            }
        }

        public bool DeleteEntry(string primaryKey)
        {
            _data.TryRemove(primaryKey, out TResource resourceRemoved);

            return resourceRemoved != default;
        }

        public IEnumerable<TResource> GetAllEntries()
        {
            return _data.Values.AsEnumerable();
        }

        public TResource GetEntryByPrimaryKey(string primaryKey)
        {
            return _data.GetValueOrDefault(primaryKey);
        }

        public TResource InsertEntry(object resource)
        {
            TResource typedResource = (TResource)resource;
            object primaryKey = GetPrimaryKeyValue(typedResource);

            return _data.TryAdd(primaryKey.ToString(), typedResource) ? typedResource : default;
        }

        public TResource UpdateEntry(object resource, string primaryKey)
        {
            TResource typedResource = (TResource)resource;

            // If we are not able to retrieve the current value and update it to the new value, return the default value
            return _data.TryGetValue(primaryKey, out TResource existingResource) && _data.TryUpdate(primaryKey, typedResource, existingResource)
                ? typedResource
                : default;
        }
    }
}
