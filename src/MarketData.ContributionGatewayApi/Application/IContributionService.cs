using MarketData.ContributionGatewayApi.Domain;

namespace MarketData.ContributionGatewayApi.Application;

public interface IContributionService
{
    Task<CreateContributionResult> CreateContribution(
        MarketDataContribution contribution,
        CancellationToken cancellationToken );
        
    Task<GetContributionsResult> GetContributions(
        CancellationToken cancellationToken );
}
