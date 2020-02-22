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
        /// <param name="valueComputationFunc">The func used to generate the value.</param>
        /// <param name="endpointTriggers">The HTTP method verbs that will trigger the computation.</param>
        public ValueComputationFuncModel(Expression<Func<object>> valueComputationFunc, HttpVerbs endpointTriggers)
        {
            ValueComputationFunc = valueComputationFunc;
            EndpointTriggers = endpointTriggers;
        }

        /// <summary>
        /// The func to generate the property value.
        /// </summary>
        public Expression<Func<object>> ValueComputationFunc { get; set; } = null;

        /// <summary>
        /// The HTTP method verbs that will trigger the computation.
        /// </summary>
        public HttpVerbs EndpointTriggers { get; set; } = HttpVerbs.POST | HttpVerbs.PUT;
    }
}
