using System.Collections.Generic;
using Insight.Tinkoff.Invest.Dto.Payloads;
using Insight.Tinkoff.Invest.Infrastructure;
using Newtonsoft.Json;

namespace Insight.Tinkoff.Invest.Dto.Responses
{
    public sealed class MarketInstrumentListResponse : ResponseBase
    {
        public int Total { get; }

        public IReadOnlyCollection<MarketInstrument> Instruments { get; }

        [JsonConstructor]
        public MarketInstrumentListResponse([JsonProperty("payload")] MarketInstrumentResponsePayload payload)
        {
            Total = payload.Total;
            Instruments = payload.Instruments;
        }
    }
}