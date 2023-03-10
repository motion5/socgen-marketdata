using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MarketData.ContributionGatewayApi.Tests.Integration;

public class ContributionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;
    private const string BaseRoute = "contribution";

    public ContributionTests( )
    {
        this.factory = new WebApplicationFactory<Program>( ).WithWebHostBuilder( builder =>
                                                                                 {
                                                                                     builder
                                                                                        .UseEnvironment( "Development" );
                                                                                 } );
    }

    [Fact]
    public async Task CreateMarketDataContribution_CreatesAContribution_WhenDataIsValid( )
    {
        // Arrange
        var client = this.factory.CreateClient( );
        var contributionRequest = GenerateContributionRequest( );

        // Act
        var response = await client.PostAsJsonAsync( BaseRoute,
                                                     contributionRequest );
        var createdContribution = await response.Content.ReadFromJsonAsync<ContributionResponse>( );

        // Assert
        response.StatusCode.Should( )
                .Be( HttpStatusCode.Created );
        createdContribution!.Id.Should( )
                            .NotBe( Guid.Empty );
        createdContribution.MarketDataType.Should( )
                           .Be( contributionRequest.MarketDataType );
        createdContribution.MarketData.Should( )
                           .BeEquivalentTo( contributionRequest.MarketData );
        createdContribution.CreatedDate.Should( )
                           .BeCloseTo( DateTime.UtcNow,
                                       new TimeSpan( 0,
                                                     0,
                                                     0,
                                                     1 ) );
    }

    private ContributionRequest GenerateContributionRequest( )
    {
        return new("FxQuote",
                   new("EUR/USD",
                       1.1234m,
                       1.1236m)
                  );
    }

    /**
     * POST /api/contribution
{
    "marketDataType": "FxQuote",
    "marketData": {
        "currencyPair": "EUR/USD",
        "bid": 1.1234,
        "ask": 1.1236
    }
}
     */
    public record ContributionRequest( string MarketDataType,
                                       MarketData MarketData );

    public record MarketData( string CurrencyPair,
                              decimal Bid,
                              decimal Ask );

    /**
     * Returns 
{
    "id": "123",
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
}
