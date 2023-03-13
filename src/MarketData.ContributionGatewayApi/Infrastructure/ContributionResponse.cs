namespace MarketData.ContributionGatewayApi.Infrastructure;

/**
     * Returns 
{
    "id": "00000000-0000-0000-0000-000000000000",
    "marketDataType": "FxQuote",
    "marketData": {
        "currencyPair": "EUR/USD",
        "bid": 1.1234,
        "ask": 1.1236
    },
    "status": "Validated"
    "createdDate": "2020-01-01T00:00:00Z"
}
     */
public record ContributionResponse( Guid Id,
                                    string MarketDataType,
                                    MarketData MarketData,
                                    string Status,
                                    DateTime CreatedDate );
