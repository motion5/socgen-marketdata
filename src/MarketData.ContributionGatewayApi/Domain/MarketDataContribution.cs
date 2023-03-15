using LanguageExt;

namespace MarketData.ContributionGatewayApi.Domain;

public class MarketDataContribution
{
    public MarketDataContribution(
        MarketDataType marketDataType,
        Infrastructure.MarketDataValue marketData,
        MarketDataContributionStatus status,
        DateTime createdDate )
    {
        this.MarketDataType = marketDataType;
        this.MarketData = marketData;
        this.Status = status;
        this.CreatedDate = createdDate;
    }

    public string? Id { get; set; }
    public MarketDataType MarketDataType { get; }
    public Infrastructure.MarketDataValue MarketData { get; }
    public MarketDataContributionStatus Status { get; set; }
    public DateTime CreatedDate { get; }
    
    public MarketDataContribution SetStatus( MarketDataContributionStatus status )
    {
        this.Status = status;
        return this;
    }

    public static Either<ValidationError, MarketDataContribution> Create(
        string marketDataType,
        Infrastructure.MarketDataValue marketData )
    {
        var validationErrors = new List<string>( );

        // validate
        if ( string.IsNullOrWhiteSpace( marketDataType ) )
        {
            validationErrors.Add( $"{nameof( marketDataType )} cannot be null or empty" );
        }

        if ( string.IsNullOrWhiteSpace( marketData.CurrencyPair ) )
        {
            validationErrors.Add( $"{nameof( marketData.CurrencyPair )} cannot be null or empty" );
        }

        if ( !Enum.IsDefined( typeof( MarketDataType ),
                              marketDataType ) )
        {
            validationErrors.Add( $"{nameof( marketDataType )} is not a valid market data type" );
        }

        if ( validationErrors.Any( ) )
        {
            return Either<ValidationError, MarketDataContribution>
               .Left( new ValidationError( validationErrors ) );
        }

        // parse
        var market = (MarketDataType)Enum.Parse( typeof( MarketDataType ),
                                                 marketDataType );

        return Either<ValidationError, MarketDataContribution>
           .Right( new(market,
                       marketData,
                       MarketDataContributionStatus.NotValidated,
                       DateTime.UtcNow) );
    }
}
