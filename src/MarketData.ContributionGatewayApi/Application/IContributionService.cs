using LanguageExt;
using MarketData.ContributionGatewayApi.Domain;

namespace MarketData.ContributionGatewayApi.Application;

public interface IContributionService
{
    Task<Either<ApplicationError, MarketDataContribution>> CreateContribution(
        MarketDataContribution contribution,
        CancellationToken cancellationToken );
        
    Task<GetContributionsResult> GetContributions(
        CancellationToken cancellationToken );
}
