using System;

namespace Snoozle.Abstractions.Models
{
    /// <summary>
    /// An action that will be called on a property during certain write operations.
    /// </summary>
    public class ValueComputationActionModel
    {
        /// <summary>
        /// initialises a new instance of the <see cref="ValueComputationAction"/> class.
        /// </summary>
        /// <param name="valueComputationAction">The action to apply to the property.</param>
        /// <param name="endpointTriggers">The HTTP method verbs that will trigger the action.</param>
        public ValueComputationActionModel(Action<object> valueComputationAction, HttpVerb endpointTriggers)
        {
            ValueComputationAction = valueComputationAction;
            EndpointTriggers = endpointTriggers;
        }

        /// <summary>
        /// The Action to be called on the property.
        /// </summary>
        public Action<object> ValueComputationAction { get; set; } = null;

        /// <summary>
        /// The HTTP method verbs that will trigger the action.
        /// </summary>
        public HttpVerb EndpointTriggers { get; set; } = HttpVerb.POST | HttpVerb.PUT;
    }
}
