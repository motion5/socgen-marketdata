using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using LanguageExt;
using MarketData.ContributionGatewayApi.Domain;
using MarketData.ContributionGatewayApi.Infrastructure;
using NSubstitute;
using Xunit;

namespace MarketData.ContributionGatewayApi.Tests.Integration;

public class ContributionTests : IClassFixture<ContributionApiFactory>
{
    private const string BaseRoute = "contribution";
    private readonly ContributionApiFactory fixture;
    private readonly JsonSerializerOptions
        jsonSerializerOptions = new( )
                                {
                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                    Converters =
                                    {
                                        new JsonStringEnumConverter( JsonNamingPolicy.CamelCase )
                                    }
                                };
    public ContributionTests( ContributionApiFactory fixture )
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task CreateMarketDataContribution_CreatesAContribution_WhenDataIsValid( )
    {
        // Arrange
        var sut = this.fixture.CreateClient( );
        var contributionRequest = GenerateContributionRequest( "FxQuote",
                                                               "EUR/USD",
                                                               1.2345m,
                                                               1.2346m );

        this.fixture.ValidationService.Validate( Arg.Any<MarketDataContribution>( ),
                                                 Arg.Any<CancellationToken>( ) )
            .Returns( Task.FromResult( Either<ApplicationError, ValidationServiceSuccess>
                                          .Right( new ValidationServiceSuccess( ) )
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
        var sut = this.fixture.CreateClient( );
        var contributionRequest = GenerateContributionRequest( "InvalidType",
                                                               "EUR/USD",
                                                               1.2345m,
                                                               1.2346m );

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
    
    [Fact]
    public async Task GetMarketDataContributions_ReturnsContributions_WhenContributionsExist( )
    {
        // Arrange
        var sut = this.fixture.CreateClient( );
        this.fixture.ValidationService.Validate( Arg.Any<MarketDataContribution>( ),
                                                 Arg.Any<CancellationToken>( ) )
            .Returns( Task.FromResult( Either<ApplicationError, ValidationServiceSuccess>
                                          .Right( new ValidationServiceSuccess( ) )
                                     )
                    );

        var createContributionResponse = await sut.PostAsJsonAsync( BaseRoute,
                                                       GenerateContributionRequest( "FxQuote",
                                                                                    "EUR/USD",
                                                                                    1.2345m,
                                                                                    1.2346m ) );
        createContributionResponse.EnsureSuccessStatusCode( );
        
        var createdRecord =
            await createContributionResponse.Content
                                            .ReadFromJsonAsync<MarketDataContribution>( jsonSerializerOptions );
        
        // Act
        var response = await sut.GetAsync( BaseRoute, CancellationToken.None );

        // Assert
        response.EnsureSuccessStatusCode( );

        var createdContributions =
            await response.Content
                          .ReadFromJsonAsync<IEnumerable<MarketDataContribution>>( jsonSerializerOptions );

        response.StatusCode.Should( )
                .Be( HttpStatusCode.OK );
        
        // added to be greater than or equal because other test could add 
        // record and I don't have an endpoint to remove it
        // In real solution that would exist or you could implement teardown
        createdContributions.Should( )
                            .HaveCountGreaterThanOrEqualTo( 1 );
        
        // the first record should be the one we just created
        createdContributions.First( )
                            .Id.Should( )
                            .NotBeEmpty( );
        
        createdContributions.First( )
                            .MarketDataType.ToString( )
                            .Should( )
                            .Be( createdRecord.MarketDataType.ToString( ) );
        
        createdContributions.First( )
                            .MarketData.Should( )
                            .BeEquivalentTo( createdRecord.MarketData );
    }

    public ContributionRequest GenerateContributionRequest( string marketDataType,
                                                            string currencyPair,
                                                            decimal bid,
                                                            decimal ask )
    {
        return new ContributionRequest( marketDataType,
                                        new MarketDataValue( currencyPair,
                                                             bid,
                                                             ask )
                                      );
    }

    public record ContributionRequest( string MarketDataType,
                                       MarketDataValue MarketData );
}
