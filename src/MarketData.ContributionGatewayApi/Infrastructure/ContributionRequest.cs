using MarketData.ContributionGatewayApi.Domain;

namespace MarketData.ContributionGatewayApi.Infrastructure;

public record ContributionRequest( string MarketDataType,
                                   MarketData MarketData )
{
    public string Id { get; set; } 
    // application will only take on domain object
    public MarketDataContribution ToDomainModel( )
    {
        return MarketDataContribution.Create( MarketDataType, MarketData );
    }
};

public record MarketData( string CurrencyPair,
                          decimal Bid,
                          decimal Ask );

