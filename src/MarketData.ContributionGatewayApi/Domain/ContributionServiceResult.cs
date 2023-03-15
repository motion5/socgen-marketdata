using OneOf;

namespace MarketData.ContributionGatewayApi.Domain;

public class ContributionServiceResult :
    OneOfBase<MarketDataContribution, ValidationError, DatabaseError>
{
    protected ContributionServiceResult(
        OneOf<MarketDataContribution, ValidationError, DatabaseError> input ) :
        base( input ) { }

    public static implicit operator ContributionServiceResult( MarketDataContribution success )
        => new(success);

    public static implicit operator ContributionServiceResult( ValidationError fail )
        => new(fail);

    public static implicit operator ContributionServiceResult( DatabaseError fail )
        => new(fail);
}
