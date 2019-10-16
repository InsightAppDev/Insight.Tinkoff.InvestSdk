using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Insight.Tinkoff.Invest.Domain;
using Insight.Tinkoff.Invest.Dto.Messages;
using Insight.Tinkoff.Invest.Infrastructure;
using Insight.Tinkoff.Invest.Infrastructure.Configurations;
using Insight.Tinkoff.Invest.Infrastructure.Json;
using PureWebSockets;

namespace Insight.Tinkoff.Invest.Services
{
    public sealed class StreamMarketService : IStreamMarketService, IDisposable
    {
        private readonly PureWebSocket _socket;
        
        private bool Disposed { get; set; }
        
        public bool OnAir { get; private set; }

        public StreamMarketService(StreamMarketServiceConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _socket = new PureWebSocket(configuration.Address
                , new PureWebSocketOptions
                {
                    Headers = new[]
                    {
                        new Tuple<string, string>("Authorization", $"Bearer {configuration.Token}")
                    }
                });
        }

        public async Task Send(IWsMessage message)
        {
            if (!OnAir)
                Connect();

            var payload = JSerializer.Serialize(message);
            await _socket.SendAsync(payload);
        }

        public IObservable<WsMessage> AsObservable()
        {
            return Observable.FromEventPattern<Message, string>(
                    handler =>
                    {
                        Connect();

                        _socket.OnMessage += handler;
                    },
                    handler =>
                    {
                        Disconnect();

                        _socket.OnMessage -= handler;
                    })
                .Select(x => DeserializeMessage(x.EventArgs));
        }

        private void Connect()
        {
            if (!OnAir && _socket.Connect())
                OnAir = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private WsMessage DeserializeMessage(string message)
        {
            var eventType = JSerializer.Deserialize<WsMessage>(message).Event;
            switch (eventType)
            {
                case EventTypes.OrderBook:
                    return JSerializer.Deserialize<OrderBookMessage>(message);
                case EventTypes.Candle:
                    return JSerializer.Deserialize<CandleMessage>(message);
                case EventTypes.InstrumentInfo:
                    return JSerializer.Deserialize<InstrumentInfoMessage>(message);
                default:
                    throw new ArgumentException(nameof(eventType));
            }
        }

        private void Disconnect()
        {
            _socket.Disconnect();
            OnAir = false;
        }

        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    _socket?.Dispose();
                }

                Disposed = true;
            }
        }

        ~StreamMarketService()
        {
            Dispose(false);
        }
    }
}