using Insight.Tinkoff.InvestSdk.Infrastructure;
using Newtonsoft.Json;

namespace Insight.Tinkoff.InvestSdk.Dto.Responses
{
    public sealed class LimitOrderResponse : ResponseBase
    {
        [JsonProperty] 
        public PlacedOrder Order { get; }

        [JsonConstructor]
        public LimitOrderResponse([JsonProperty("payload")] PlacedOrder order)
        {
            Order = order;
        }
    }
}