using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Insight.Tinkoff.Invest.Domain;
using Insight.Tinkoff.Invest.Dto;
using Insight.Tinkoff.Invest.Services;
using Insight.Tinkoff.Invest.Tests.Base;
using Xunit;

namespace Insight.Tinkoff.Invest.Tests
{
    public sealed class OrderTest : TestBase
    {
        private readonly IOrderService _orderService;
        private readonly ISandboxService _sandboxService;

        public OrderTest()
        {
            var client = new HttpClient();
            _orderService = new OrderService(RestConfiguration, client);
            _sandboxService = new SandboxService(RestConfiguration, client);
        }

        [Fact]
        public async Task Should_get_orders()
        {
            var response = await _orderService.Get(CancellationToken.None);

            ValidateRestResponse(response);
            Assert.NotNull(response.Orders);
        }

        [Fact]
        public async Task Should_place_limit_order()
        {
            var balanceSetResponse = await _sandboxService.SetCurrencyBalance(new SandboxSetCurrencyBalanceRequest
            {
                Balance = 200,
                Currency = Currency.Usd
            });
            
            ValidateRestResponse(balanceSetResponse);
            
            var request = new PlaceLimitOrderRequest
            {
                Lots = 1,
                Operation = OperationType.Buy,
                Price = 180
            };
            
            var response = await _orderService.PlaceLimitOrder("BBG000D9D830", request, CancellationToken.None);

            ValidateRestResponse(response);
            Assert.NotNull(response.Order);
            Assert.Equal(OrderStatus.Fill, response.Order.Status);
        }
    }
}