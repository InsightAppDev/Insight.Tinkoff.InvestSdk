using System.Threading;
using System.Threading.Tasks;
using Insight.Tinkoff.Invest.Dto;
using Insight.Tinkoff.Invest.Infrastructure;

namespace Insight.Tinkoff.Invest.Domain
{
    public interface ISandboxService
    {
        Task<EmptyResponse> Register(CancellationToken cancellationToken = default);

        Task<EmptyResponse> Clear(CancellationToken cancellationToken = default);

        Task<EmptyResponse> SetCurrenciesBalance(SandboxSetCurrencyBalanceRequest request,
            CancellationToken cancellationToken = default);
        
        Task<EmptyResponse> SetPositionBalance(SandboxSetPositionBalanceRequest request,
            CancellationToken cancellationToken = default);
    }
}