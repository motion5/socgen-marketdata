using OneOf;

namespace MarketData.ContributionGatewayApi.Domain;

public class CreateContributionResult :
    OneOfBase<MarketDataContribution, ValidationError, DatabaseError>
{
    protected CreateContributionResult(
        OneOf<MarketDataContribution, ValidationError, DatabaseError> input ) :
        base( input ) { }

    public static implicit operator CreateContributionResult( MarketDataContribution success )
        => new(success);

    public static implicit operator CreateContributionResult( ValidationError fail )
        => new(fail);

    public static implicit operator CreateContributionResult( DatabaseError fail )
        => new(fail);
}
