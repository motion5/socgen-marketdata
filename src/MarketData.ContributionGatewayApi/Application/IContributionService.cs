using MarketData.ContributionGatewayApi.Domain;

namespace MarketData.ContributionGatewayApi.Application;

public interface IContributionService
{
    Task<ContributionServiceResult> CreateContribution(
        MarketDataContribution contribution,
        CancellationToken cancellationToken );
}
