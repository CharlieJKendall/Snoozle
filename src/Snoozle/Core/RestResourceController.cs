using Microsoft.AspNetCore.Mvc;
using Snoozle.Configuration;
using Snoozle.Sql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snoozle.Core
{
    [Route("api/[controller]")]
    public sealed class RestResourceController<T> : ControllerBase
        where T : class, IRestResource
    {
        private readonly ISqlExecutor _sqlExecutor;
        private readonly IRuntimeConfiguration _runtimeConfiguration;

        public RestResourceController(ISqlExecutor sqlExecutor, IRuntimeConfigurationProvider runtimeConfigurationProvider)
        {
            _sqlExecutor = sqlExecutor;
            _runtimeConfiguration = runtimeConfigurationProvider.GetRuntimeConfigurationForType(typeof(T));
        }

        [HttpPost]
        public async Task<ActionResult<T>> Post([FromBody] T resource)
        {
            return null;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<T>>> GetAll()
        {
            IEnumerable<IRestResource> results = await _sqlExecutor.ExecuteSelectAllAsync(
                _runtimeConfiguration.SqlStrings.SelectAll,
                _runtimeConfiguration.GetSqlMapToResource);

            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetById(string id)
        {
            IRestResource result = await _sqlExecutor.ExecuteSelectByIdAsync(
                _runtimeConfiguration.SqlStrings.SelectById,
                _runtimeConfiguration.GetSqlMapToResource,
                _runtimeConfiguration.GetPrimaryKeySqlParameter,
                id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<T>> Put(string id, [FromBody] T resource)
        {
            return null;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            bool success = await _sqlExecutor.ExecuteDeleteByIdAsync(
                _runtimeConfiguration.SqlStrings.DeleteById,
                _runtimeConfiguration.GetPrimaryKeySqlParameter,
                id);

            if (success)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
