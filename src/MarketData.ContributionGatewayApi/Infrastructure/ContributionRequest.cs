using MarketData.ContributionGatewayApi.Domain;
using OneOf;

namespace MarketData.ContributionGatewayApi.Infrastructure;

public record ContributionRequest( string MarketDataType,
                                   TickData TickData )
{
    public string Id { get; set; } 
    
    public MarketDataContribution ToMarketDataContribution( )
    {
        return MarketDataContribution.Create( MarketDataType, this.TickData );
    }
};

public record TickData( string CurrencyPair,
                          decimal Bid,
                          decimal Ask );

