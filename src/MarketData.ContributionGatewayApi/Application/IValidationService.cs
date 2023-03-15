using LanguageExt;
using MarketData.ContributionGatewayApi.Domain;

namespace MarketData.ContributionGatewayApi.Application;

public interface IValidationService
{
    public Task<Either<ValidationServiceFail, ValidationServiceSuccess>> Validate(
        MarketDataContribution contribution,
        CancellationToken cancellationToken );
}
