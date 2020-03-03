using System;
using System.Linq.Expressions;

namespace Snoozle.Abstractions.Models
{
    /// <summary>
    /// A func that generates a value to be applied to a property during write operations.
    /// </summary>
    public class ValueComputationFuncModel
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ValueComputationFuncModel"/> class.
        /// </summary>
        /// <param name="endpointTriggers">The HTTP method verbs that will trigger the computation.</param>
        public ValueComputationFuncModel(HttpVerbs endpointTriggers)
        {
            EndpointTriggers = endpointTriggers;
        }

        /// <summary>
        /// The func to generate the property value.
        /// </summary>
        public Expression<Func<object>> ValueComputationFunc { get; set; }

        /// <summary>
        /// The HTTP method verbs that will trigger the computation.
        /// </summary>
        public HttpVerbs EndpointTriggers { get; set; } = HttpVerbs.POST | HttpVerbs.PUT;

        public bool HasEndpointTrigger(HttpVerbs flag) => EndpointTriggers.HasFlag(flag);
    }
}
