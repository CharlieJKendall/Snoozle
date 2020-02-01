using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snoozle.Abstractions
{
    public interface IDataProvider
    {
        Task<T> ExecuteSelectByIdAsync<T>(object primaryKey)
            where T : class, IRestResource;

        Task<IEnumerable<T>> ExecuteSelectAllAsync<T>()
            where T : class, IRestResource;

        Task<bool> ExecuteDeleteByIdAsync<T>(object primaryKey)
            where T : class, IRestResource;

        Task<T> ExecuteInsertAsync<T>(T resourceToCreate)
            where T : class, IRestResource;

        Task<T> ExecuteUpdateAsync<T>(T resourceToCreate, object primaryKey)
            where T : class, IRestResource;
    }
}
