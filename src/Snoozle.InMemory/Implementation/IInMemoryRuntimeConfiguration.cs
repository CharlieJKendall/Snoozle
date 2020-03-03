using Snoozle.Abstractions;
using System.Collections.Generic;

namespace Snoozle.InMemory.Implementation
{
    public interface IInMemoryRuntimeConfiguration<out TResource> : IRuntimeConfiguration
        where TResource : class, IRestResource
    {
        TResource GetEntryByPrimaryKey(string primaryKey);

        IEnumerable<TResource> GetAllEntries();

        TResource InsertEntry(object resource);

        TResource UpdateEntry(object resource, string primaryKey);

        bool DeleteEntry(string primaryKey);
    }
}