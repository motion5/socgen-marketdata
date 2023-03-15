using LanguageExt;
using MarketData.ContributionGatewayApi.Domain;

namespace MarketData.ContributionGatewayApi.Application;

public interface IValidationService
{
    public Task<Either<ApplicationError, ValidationServiceSuccess>> Validate(MarketDataContribution contribution,
        CancellationToken cancellationToken);
}
