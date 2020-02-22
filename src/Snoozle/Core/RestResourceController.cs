using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Snoozle.Abstractions;
using Snoozle.Abstractions.Models;
using Snoozle.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Snoozle.Core
{
    [Route("api/[controller]")]
    public sealed class RestResourceController<TResource> : ControllerBase
        where TResource : class, IRestResource
    {
        private readonly IDataProvider _dataProvider;
        private readonly IRuntimeConfiguration _runtimeConfiguration;
        private readonly ILogger<RestResourceController<TResource>> _logger;
        private readonly SnoozleOptions _options;

        public RestResourceController(
            IDataProvider dataProvider,
            IRuntimeConfigurationProvider<IRuntimeConfiguration> runtimeConfigurationProvider,
            ILogger<RestResourceController<TResource>> logger,
            IOptions<SnoozleOptions> options)
        {
            _dataProvider = dataProvider;
            _runtimeConfiguration = runtimeConfigurationProvider.GetRuntimeConfigurationForType(typeof(TResource));
            _logger = logger;
            _options = options.Value;
        }

        [HttpPost]
        public async Task<ActionResult<TResource>> Post([FromBody] TResource resourceToCreate)
        {
            if (MethodIsDisallowed(HttpVerbs.POST))
            {
                return MethodNotAllowed();
            }

            if (resourceToCreate == null)
            {
                return BadRequest();
            }

            ApplyComputedValues(resourceToCreate, HttpVerbs.POST);

            try
            {
                TResource resourceCreated = await _dataProvider.InsertAsync(resourceToCreate).ConfigureAwait(false);

                return Ok(resourceCreated);
            }
            catch (NotSupportedException)
            {
                // Data providers that do not support certain HTTP methods throw NotSupportedException
                return MethodNotAllowed();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TResource>>> GetAll()
        {
            if (MethodIsDisallowed(HttpVerbs.GET))
            {
                return MethodNotAllowed();
            }

            try
            {
                IEnumerable<TResource> results = await _dataProvider.SelectAllAsync<TResource>().ConfigureAwait(false);

                return Ok(results);
            }
            catch (NotSupportedException)
            {
                // Data providers that do not support certain HTTP methods throw NotSupportedException
                return MethodNotAllowed();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TResource>> GetById(string id)
        {
            if (MethodIsDisallowed(HttpVerbs.GET))
            {
                return MethodNotAllowed();
            }

            try
            {
                TResource result = await _dataProvider.SelectByIdAsync<TResource>(id).ConfigureAwait(false);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (NotSupportedException)
            {
                // Data providers that do not support certain HTTP methods throw NotSupportedException
                return MethodNotAllowed();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TResource>> Put(string id, [FromBody] TResource resourceToUpdate)
        {
            if (MethodIsDisallowed(HttpVerbs.PUT))
            {
                return MethodNotAllowed();
            }

            if (resourceToUpdate == null || _runtimeConfiguration.GetPrimaryKeyValue(resourceToUpdate).ToString() != id)
            {
                return BadRequest();
            }

            ApplyComputedValues(resourceToUpdate, HttpVerbs.PUT);

            try
            {
                TResource resourceUpdated = await _dataProvider.UpdateAsync(resourceToUpdate, id).ConfigureAwait(false);

                if (resourceUpdated == null)
                {
                    return NotFound();
                }

                return Ok(resourceUpdated);
            }
            catch (NotSupportedException)
            {
                // Data providers that do not support certain HTTP methods throw NotSupportedException
                return MethodNotAllowed();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            if (MethodIsDisallowed(HttpVerbs.DELETE))
            {
                return MethodNotAllowed();
            }

            try
            {
                bool success = await _dataProvider.DeleteByIdAsync<TResource>(id).ConfigureAwait(false);

                if (success)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (NotSupportedException)
            {
                // Data providers that do not support certain HTTP methods throw NotSupportedException
                return MethodNotAllowed();
            }
        }

        private ActionResult MethodNotAllowed()
        {
            _logger.LogInformation("HTTP method {HttpMethod} is not allowed for path {UrlPath}", HttpContext.Request.Method, HttpContext.Request.Path.Value);

            return StatusCode((int)HttpStatusCode.MethodNotAllowed);
        }

        private bool MethodIsDisallowed(HttpVerbs httpVerb)
        {
            var disallowedGlobally = (_options.AllowedVerbs & httpVerb) != httpVerb;
            var disallowedOnResource = (_runtimeConfiguration.AllowedVerbsFlags & httpVerb) != httpVerb;

            return disallowedGlobally || disallowedOnResource;
        }

        private void ApplyComputedValues(TResource resource, HttpVerbs httpVerb)
        {
            foreach (ValueComputationActionModel action in _runtimeConfiguration.ValueComputationActions)
            {
                // Only apply the computed value if the user has specified that it is to be applied for the given HTTP verb
                // i.e. some values are only updated for POST (e.g. DateCreated)
                if ((action.EndpointTriggers & httpVerb) == httpVerb)
                {
                    action.ValueComputationAction(resource);
                }
            }
        }
    }
}
