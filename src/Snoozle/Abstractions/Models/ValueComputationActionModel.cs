using System;

namespace Snoozle.Abstractions.Models
{
    public class ValueComputationActionModel
    {
        public ValueComputationActionModel(Action<object> valueComputationAction, HttpVerb endpointTriggers)
        {
            ValueComputationAction = valueComputationAction;
            EndpointTriggers = endpointTriggers;
        }

        public Action<object> ValueComputationAction { get; set; } = null;

        public HttpVerb EndpointTriggers { get; set; } = HttpVerb.POST | HttpVerb.PUT;
    }
}
