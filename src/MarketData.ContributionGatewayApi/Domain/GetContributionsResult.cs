using OneOf;

namespace MarketData.ContributionGatewayApi.Domain;

public class GetContributionsResult :
    OneOfBase<MarketDataContributions, DatabaseError>
{
    protected GetContributionsResult(
        OneOf<MarketDataContributions, DatabaseError> input ) : base( input ) { }

    public static implicit operator GetContributionsResult(
        MarketDataContributions success )
        => new(success);

    public static implicit operator GetContributionsResult( DatabaseError fail ) => new(fail);
}
