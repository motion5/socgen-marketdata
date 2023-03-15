using LanguageExt;
using MarketData.ContributionGatewayApi.Domain;

namespace MarketData.ContributionGatewayApi.Infrastructure;

public record ContributionRequest( string MarketDataType,
                                   MarketDataValue MarketData )
{
    public string? Id { get; set; }

    public Either<ValidationError, MarketDataContribution> ToMarketDataContribution( )
        => MarketDataContribution.Create( MarketDataType,
                                          MarketData );
};

public record MarketDataValue( string CurrencyPair,
                               decimal Bid,
                               decimal Ask );
