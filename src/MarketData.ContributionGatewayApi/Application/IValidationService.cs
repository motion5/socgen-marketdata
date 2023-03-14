using MarketData.ContributionGatewayApi.Domain;
using OneOf;

namespace MarketData.ContributionGatewayApi.Application;

public interface IValidationService
{
    public Task<OneOf<MarketDataContribution, ValidationError>> Validate( MarketDataContribution contribution );
}
