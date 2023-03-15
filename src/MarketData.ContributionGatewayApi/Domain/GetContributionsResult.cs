using OneOf;

namespace MarketData.ContributionGatewayApi.Domain;

public class GetContributionsResult :
    OneOfBase<List<MarketDataContribution>, DatabaseError>
{
    protected GetContributionsResult(
        OneOf<List<MarketDataContribution>, DatabaseError> input ) : base( input ) { }

    public static implicit operator GetContributionsResult(
        List<MarketDataContribution> success )
        => new(success);

    public static implicit operator GetContributionsResult( DatabaseError fail ) => new(fail);
}
