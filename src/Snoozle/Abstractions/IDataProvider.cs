using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snoozle.Abstractions
{
    public interface IDataProvider
    {
        Task<T> SelectByIdAsync<T>(object primaryKey)
            where T : class, IRestResource;

        Task<IEnumerable<T>> SelectAllAsync<T>()
            where T : class, IRestResource;

        Task<bool> DeleteByIdAsync<T>(object primaryKey)
            where T : class, IRestResource;

        Task<T> InsertAsync<T>(T resourceToCreate)
            where T : class, IRestResource;

        Task<T> UpdateAsync<T>(T resourceToCreate, object primaryKey)
            where T : class, IRestResource;
    }
}
