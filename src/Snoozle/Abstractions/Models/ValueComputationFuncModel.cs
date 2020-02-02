using System;
using System.Linq.Expressions;

namespace Snoozle.Abstractions.Models
{
    public class ValueComputationFuncModel
    {
        public ValueComputationFuncModel(Expression<Func<object>> valueComputationFunc, HttpVerb endpointTriggers)
        {
            ValueComputationFunc = valueComputationFunc;
            EndpointTriggers = endpointTriggers;
        }

        public Expression<Func<object>> ValueComputationFunc { get; set; } = null;

        public HttpVerb EndpointTriggers { get; set; } = HttpVerb.POST | HttpVerb.PUT;
    }
}
