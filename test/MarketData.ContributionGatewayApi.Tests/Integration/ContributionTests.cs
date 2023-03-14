using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using MarketData.ContributionGatewayApi.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SurrealDb.Client;
using SurrealDb.Client.SurrealResponses;
using Xunit;

namespace MarketData.ContributionGatewayApi.Tests.Integration;

public class ContributionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;
    private const string BaseRoute = "contribution";
    private readonly ISurrealDbClient surrealDbClient = Substitute.For<ISurrealDbClient>( );

    private readonly JsonSerializerOptions
        jsonSerializerOptions = new( )
                                {
                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                    Converters =
                                    {
                                        new JsonStringEnumConverter( JsonNamingPolicy.CamelCase )
                                    }
                                };

    public ContributionTests( )
    {
        this.factory =
            new WebApplicationFactory<Program>( )
               .WithWebHostBuilder(
                                   builder =>
                                   {
                                       builder
                                          .UseEnvironment( "Development" );
                                       builder
                                          .ConfigureServices( services
                                                                  =>
                                                              {
                                                                  services.AddJsonConverters( );
                                                                  services
                                                                     .AddTransient
                                                                          <ISurrealDbClient>( _ => this
                                                                                                 .surrealDbClient );
                                                              } );
                                   } );
    }

    [Fact]
    public async Task CreateMarketDataContribution_CreatesAContribution_WhenDataIsValid( )
    {
        // Arrange
        var sut = this.factory.CreateClient( );
        var contributionRequest = GenerateContributionRequest( "FxQuote",
                                                               "EUR/USD",
                                                               1.2345m,
                                                               1.2346m );
        this.surrealDbClient.CreateRecord( Arg.Any<MarketDataContribution>( ),
                                           Arg.Any<CancellationToken>( ) )
            .Returns( Task.FromResult( GenerateCreateMarketDataContributionResponse( "FxQuote",
                                                                                     "EUR/USD",
                                                                                     1.2345m,
                                                                                     1.2346m
                                                                                   )
                                     )
                    );

        // Act
        var response = await sut.PostAsJsonAsync( BaseRoute,
                                                  contributionRequest );

        // Assert
        response.EnsureSuccessStatusCode( );

        var createdContribution =
            await response.Content
                          .ReadFromJsonAsync<MarketDataContribution>( jsonSerializerOptions );

        response.StatusCode.Should( )
                .Be( HttpStatusCode.Created );
        createdContribution!.Id.Should( )
                            .NotBeEmpty( );
        createdContribution.MarketDataType.ToString( )
                           .Should( )
                           .Be( contributionRequest.MarketDataType );
        createdContribution.MarketData.Should( )
                           .BeEquivalentTo( contributionRequest.MarketData );
        createdContribution.CreatedDate.Should( )
                           .BeCloseTo( DateTime.UtcNow,
                                       new TimeSpan( 0,
                                                     0,
                                                     0,
                                                     1 ) );

        response.Headers.Location.Should( )
                .Be( $"{BaseRoute}/{createdContribution.Id}" );
    }

    [Fact]
    public async Task CreateMarketDataContribution_ReturnsBadRequest_WhenDataIsInvalid( )
    {
        // Arrange
        var sut = this.factory.CreateClient( );
        var contributionRequest = GenerateContributionRequest( "InvalidType",
                                                               "EUR/USD",
                                                               1.2345m,
                                                               1.2346m );

        this.surrealDbClient.CreateRecord( Arg.Any<MarketDataContribution>( ),
                                           Arg.Any<CancellationToken>( ) )
            .Returns( Task.FromResult( new ApiResponse<MarketDataContribution>( ) ) );

        // Act
        var response = await sut.PostAsJsonAsync( BaseRoute,
                                                  contributionRequest );

        // Assert
        response.StatusCode.Should( )
                .Be( HttpStatusCode.BadRequest );
        
        
        
        var responseBody = await response.Content.ReadFromJsonAsync<ValidationError>( );
        
        responseBody.Errors.Should( )
                    .Contain( "marketDataType is not a valid market data type" ); 
    }

    public ContributionRequest GenerateContributionRequest( string marketDataType,
                                                            string currencyPair,
                                                            decimal bid,
                                                            decimal ask )
    {
        return new ContributionRequest( marketDataType,
                                        new Infrastructure.MarketDataValue( currencyPair,
                                                                            bid,
                                                                            ask )
                                      );
    }

    public record ContributionRequest( string MarketDataType,
                                       Infrastructure.MarketDataValue MarketData );


    public static ApiResponse<MarketDataContribution> GenerateCreateMarketDataContributionResponse(
        string marketDataType,
        string currencyPair,
        decimal bid,
        decimal ask )
    {
        var result =
            MarketDataContribution.Create( marketDataType,
                                           new Infrastructure.MarketDataValue( currencyPair,
                                                                               bid,
                                                                               ask )
                                         );

        var marketDataContribution = result.Match( val => val,
                                                   err => throw new Exception( err.Errors
                                                                                  .First( ) ) );

        marketDataContribution.Id = Guid.NewGuid( )
                                        .ToString( );

        return GenerateSurrealCreatedApiResponse( marketDataContribution );
    }

    public static ApiResponse<T> GenerateSurrealCreatedApiResponse<T>( T createdRecord )
    {
        var listOfMarketDataContributions =
            new List<T> { createdRecord };
        var queryResponses = new List<QueryResponse<T>>
                             {
                                 new(listOfMarketDataContributions,
                                     "",
                                     StatusResult.OK)
                             };

        var record =
            new ApiResponse<T>( HttpStatusCode.Created )
               .SetQueryResponse( queryResponses );

        return record;
    }
}
