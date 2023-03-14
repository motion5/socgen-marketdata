using MarketData.ContributionGatewayApi.Domain;
using OneOf;

namespace MarketData.ContributionGatewayApi.Application;

public interface IContributionService
{
    Task<OneOf<MarketDataContribution, ValidationError, DatabaseError>> CreateContribution(
        MarketDataContribution domainModel,
        CancellationToken cancellationToken );
}
