namespace MarketData.ContributionGatewayApi.Domain;

public class MarketDataContribution
{
    public MarketDataContribution(
        MarketDataType marketDataType,
        Infrastructure.MarketData marketData,
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
    public Infrastructure.MarketData MarketData { get; }
    public MarketDataContributionStatus Status { get; }
    public DateTime CreatedDate { get; }

    public static MarketDataContribution Create(
        string marketDataType,
        Infrastructure.MarketData marketData )
    {
        // validate
        if ( string.IsNullOrWhiteSpace( marketDataType ) )
        {
            throw new ArgumentNullException( $"{nameof(marketDataType)} cannot be null or empty" );
        }
        if ( string.IsNullOrWhiteSpace( marketData.CurrencyPair ) )
        {
            throw new ArgumentNullException( $"{nameof(marketDataType)} cannot be null or empty" );
        }

        if ( !Enum.IsDefined( typeof( MarketDataType ),
                              marketDataType ) )
        {
            throw new ArgumentException( "Invalid market data type" );
        }

        // parse
        var market = (MarketDataType)Enum.Parse( typeof( MarketDataType ),
                                                 marketDataType );

        return new(market,
                   marketData,
                   MarketDataContributionStatus.NotValidated,
                   DateTime.UtcNow);
    }
}

public enum MarketDataContributionStatus
{
    NotValidated,
    FailedValidation,
    Validated,
}

public enum MarketDataType
{
    FxQuote,
    Future,
}
