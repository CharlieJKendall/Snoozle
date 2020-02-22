using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snoozle.Abstractions
{
    /// <summary>
    /// Provides data that can be accessed via the REST endpoints.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Provides data for the <see cref="Core.RestResourceController{TResource}.GetById(string)"/> endpoint.
        /// </summary>
        Task<T> SelectByIdAsync<T>(object primaryKey)
            where T : class, IRestResource;

        /// <summary>
        /// Provides data for the <see cref="Core.RestResourceController{TResource}.GetAll"/> endpoint.
        /// </summary>
        Task<IEnumerable<T>> SelectAllAsync<T>()
            where T : class, IRestResource;

        /// <summary>
        /// Provides data for the <see cref="Core.RestResourceController{TResource}.Delete(string)"/> endpoint.
        /// </summary>
        Task<bool> DeleteByIdAsync<T>(object primaryKey)
            where T : class, IRestResource;

        /// <summary>
        /// Provides data for the <see cref="Core.RestResourceController{TResource}.Post(TResource)"/> endpoint.
        /// </summary>
        Task<T> InsertAsync<T>(T resourceToCreate)
            where T : class, IRestResource;

        /// <summary>
        /// Provides data for the <see cref="Core.RestResourceController{TResource}.Put(string, TResource)"/> endpoint.
        /// </summary>
        Task<T> UpdateAsync<T>(T resourceToCreate, object primaryKey)
            where T : class, IRestResource;
    }
}
