using System;
using Insight.Tinkoff.Invest.Dto.Payloads;

namespace Insight.Tinkoff.Invest.Dto.Messages
{
    public sealed class UnsubscribeCandleMessage : IWsMessage
    {
        public string Event => EventType.UnubscribeCandle;
        
        public UnsubscribeCandleMessage(string figi, CandleInterval interval)
        {
            if (string.IsNullOrWhiteSpace(figi))
                throw new ArgumentNullException(nameof(figi));

            Figi = figi;
            Interval = Interval;
        }
        
        public string Figi { get; private set; }
        
        public CandleInterval Interval { get; private set; }
    }
}