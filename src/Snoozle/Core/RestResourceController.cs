using Microsoft.AspNetCore.Mvc;
using Snoozle.Abstractions;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Snoozle.Core
{
    [Route("api/[controller]")]
    public sealed class RestResourceController<T> : ControllerBase
        where T : class, IRestResource
    {
        private readonly IDataProvider _dataProvider;
        private readonly IRuntimeConfiguration _runtimeConfiguration;

        public RestResourceController(IDataProvider dataProvider, IRuntimeConfigurationProvider<IRuntimeConfiguration> runtimeConfigurationProvider)
        {
            _dataProvider = dataProvider;
            _runtimeConfiguration = runtimeConfigurationProvider.GetRuntimeConfigurationForType(typeof(T));
        }

        [HttpPost]
        public async Task<ActionResult<T>> Post([FromBody] T resourceToCreate)
        {
            if ((_runtimeConfiguration.AllowedVerbsFlags & HttpVerb.POST) != HttpVerb.POST)
            {
                return MethodNotAllowed();
            }

            if (resourceToCreate == null)
            {
                return BadRequest();
            }

            IRestResource resourceCreated = await _dataProvider.ExecuteInsertAsync(resourceToCreate);

            return Ok(resourceCreated);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<T>>> GetAll()
        {
            if ((_runtimeConfiguration.AllowedVerbsFlags & HttpVerb.GET) != HttpVerb.GET)
            {
                return MethodNotAllowed();
            }

            IEnumerable<IRestResource> results = await _dataProvider.ExecuteSelectAllAsync<T>();

            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetById(string id)
        {
            if ((_runtimeConfiguration.AllowedVerbsFlags & HttpVerb.GET) != HttpVerb.GET)
            {
                return MethodNotAllowed();
            }

            IRestResource result = await _dataProvider.ExecuteSelectByIdAsync<T>(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<T>> Put(string id, [FromBody] T resourceToUpdate)
        {
            if ((_runtimeConfiguration.AllowedVerbsFlags & HttpVerb.PUT) != HttpVerb.PUT)
            {
                return MethodNotAllowed();
            }

            if (resourceToUpdate == null || _runtimeConfiguration.GetPrimaryKeyValue(resourceToUpdate).ToString() != id)
            {
                return BadRequest();
            }

            IRestResource resourceUpdated = await _dataProvider.ExecuteUpdateAsync(resourceToUpdate, id);

            if (resourceUpdated == null)
            {
                return NotFound();
            }

            return Ok(resourceUpdated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            if ((_runtimeConfiguration.AllowedVerbsFlags & HttpVerb.DELETE) != HttpVerb.DELETE)
            {
                return MethodNotAllowed();
            }

            bool success = await _dataProvider.ExecuteDeleteByIdAsync<T>(id);

            if (success)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        private ActionResult MethodNotAllowed()
        {
            return StatusCode((int)HttpStatusCode.MethodNotAllowed);
        }
    }
}
