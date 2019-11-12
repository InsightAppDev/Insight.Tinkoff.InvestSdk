using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Insight.Tinkoff.Invest.Domain;
using Insight.Tinkoff.Invest.Dto.Messages;
using Insight.Tinkoff.Invest.Infrastructure.Configurations;
using Insight.Tinkoff.Invest.Infrastructure.Extensions;
using Insight.Tinkoff.Invest.Services;
using Insight.Tinkoff.Invest.Tests.Base;
using Xunit;
using Xunit.Abstractions;

namespace Insight.Tinkoff.Invest.Tests
{
    public sealed class StreamServiceTest : TestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private IStreamMarketService _streamService;

        public StreamServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _streamService = new StreamMarketService(new StreamConfiguration
            {
                AccessToken = Token
            });
        }

        [Fact]
        public async Task Should_receive_messages_from_as_observable()
        {
            var subscription = _streamService
                .AsObservable()
                .Do(x =>
                {
                    Assert.NotNull(x);
                    Assert.Equal("orderbook", x.Event, StringComparer.OrdinalIgnoreCase);
                    switch (x)
                    {
                        case OrderBookMessage m:
                            _testOutputHelper.WriteLine(
                                $"[{DateTime.Now:HH:mm:ss.fff}]: type: {m.Event}, figi: {m.Payload.Figi}");
                            break;
                        default:
                            throw new ArgumentException(nameof(x));
                    }
                }, ex => { throw ex; })
                .Subscribe();

            await _streamService.Send(new SubscribeOrderBookMessage("BBG000D9D830", 5));

            await Task.Delay(5 * 1000);
        }
    }
}