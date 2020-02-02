using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Snoozle.Abstractions;
using Snoozle.Configuration;
using Snoozle.Enums;
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
        private readonly ILogger<RestResourceController<T>> _logger;
        private readonly SnoozleOptions _options;

        public RestResourceController(
            IDataProvider dataProvider,
            IRuntimeConfigurationProvider<IRuntimeConfiguration> runtimeConfigurationProvider,
            ILogger<RestResourceController<T>> logger,
            IOptions<SnoozleOptions> options)
        {
            _dataProvider = dataProvider;
            _runtimeConfiguration = runtimeConfigurationProvider.GetRuntimeConfigurationForType(typeof(T));
            _logger = logger;
            _options = options.Value;
        }

        [HttpPost]
        public async Task<ActionResult<T>> Post([FromBody] T resourceToCreate)
        {
            if (MethodIsDisallowed(HttpVerb.POST))
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
            if (MethodIsDisallowed(HttpVerb.GET))
            {
                return MethodNotAllowed();
            }

            IEnumerable<IRestResource> results = await _dataProvider.ExecuteSelectAllAsync<T>();

            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetById(string id)
        {
            if (MethodIsDisallowed(HttpVerb.GET))
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
            if (MethodIsDisallowed(HttpVerb.PUT))
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
            if (MethodIsDisallowed(HttpVerb.DELETE))
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
            _logger.LogInformation("HTTP method {HttpMethod} is not allowed for path {UrlPath}", HttpContext.Request.Method, HttpContext.Request.Path.Value);

            return StatusCode((int)HttpStatusCode.MethodNotAllowed);
        }

        private bool MethodIsDisallowed(HttpVerb httpVerb)
        {
            var disallowedGlobally = (_options.AllowedVerbs & httpVerb) != httpVerb;
            var disallowedOnResource = (_runtimeConfiguration.AllowedVerbsFlags & httpVerb) != httpVerb;

            return disallowedGlobally || disallowedOnResource;
        }
    }
}
