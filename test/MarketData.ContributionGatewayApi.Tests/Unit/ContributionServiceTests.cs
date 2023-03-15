using System.Net;
using FluentAssertions;
using MarketData.ContributionGatewayApi.Application;
using MarketData.ContributionGatewayApi.Domain;
using MarketData.ContributionGatewayApi.Infrastructure;
using NSubstitute;
using SurrealDb.Client;
using SurrealDb.Client.SurrealResponses;
using Xunit;

namespace MarketData.ContributionGatewayApi.Tests.Unit;

public class ContributionServiceTests
{
    private readonly ISurrealDbClient surrealDbClient = Substitute.For<ISurrealDbClient>();
    private readonly IValidationService validationService = Substitute.For<IValidationService>();

    [Fact]
    public async Task CreateContribution_GivenMarketDataContribution_ShouldPersistContribution()
    {
        // Arrange
        this.surrealDbClient.CreateRecord(Arg.Any<MarketDataContribution>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(GenerateCreateMarketDataContributionResponse("FxQuote",
                "EUR/USD",
                1.2345m,
                1.2346m)));
        this.surrealDbClient.UpdateRecord(Arg.Any<MarketDataContribution>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(GenerateCreateMarketDataContributionResponse("FxQuote",
                "EUR/USD",
                1.2345m,
                1.2346m)));
        this.validationService.Validate(Arg.Any<MarketDataContribution>(),
                Arg.Any<CancellationToken>())
            .Returns(new ValidationServiceSuccess());

        var sut = new ContributionService(this.surrealDbClient,
            this.validationService);

        var contributionRequest = MarketDataContribution.Create("FxQuote",
                new
                    MarketDataValue("EUR/USD",
                        1.2345m,
                        1.2346m))
            .GetRight();

        // Act
        await sut.CreateContribution(contributionRequest,
            CancellationToken.None);

        // Assert
        await this.surrealDbClient.Received(1)
            .CreateRecord(Arg.Any<MarketDataContribution>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task
        CreateContribution_GivenMarketDataContribution_ShouldReturnCreatedContribution()
    {
        // Arrange
        this.surrealDbClient.CreateRecord(Arg.Any<MarketDataContribution>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(GenerateCreateMarketDataContributionResponse("FxQuote",
                "EUR/USD",
                1.2345m,
                1.2346m)));
        this.surrealDbClient.UpdateRecord(Arg.Any<MarketDataContribution>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(GenerateCreateMarketDataContributionResponse("FxQuote",
                "EUR/USD",
                1.2345m,
                1.2346m)));

        this.validationService.Validate(Arg.Any<MarketDataContribution>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationServiceSuccess());

        var marketDataRequest = MarketDataContribution.Create("FxQuote", new
                    MarketDataValue("EUR/USD",
                        1.2345m,
                        1.2346m)).GetRight();
        
        var sut = new ContributionService(this.surrealDbClient, this.validationService);

        // Act
        var result = await sut.CreateContribution(marketDataRequest, CancellationToken.None);

        // Assert
        result.Match<MarketDataContribution>(record =>
            {
                record.MarketDataType.ToString()
                    .Should()
                    .Be("FxQuote");
                record.MarketData.CurrencyPair.Should()
                    .Be("EUR/USD");
                record.MarketData.Bid.Should()
                    .Be(1.2345m);
                record.MarketData.Ask.Should()
                    .Be(1.2346m);
                return record;
            },
            _ => throw new Exception("Should not be a validation error"));
    }


    [Fact]
    public async Task CreateContribution_GivenDatabaseErrorResponse_ShouldReturnDatabaseError()
    {
        // Arrange
        this.surrealDbClient.CreateRecord(Arg.Any<MarketDataContribution>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(GenerateErroredSurrealApiResponse(MarketDataContribution
                .Create("FxQuote",
                    new MarketDataValue("EUR/USD",
                        1.2345m,
                        1.2346m)
                )
                .GetRight())));

        this.surrealDbClient.UpdateRecord(Arg.Any<MarketDataContribution>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(GenerateCreateMarketDataContributionResponse("FxQuote",
                "EUR/USD",
                1.2345m,
                1.2346m)));

        var marketDataContribution = MarketDataContribution.Create("FxQuote",
                new
                    MarketDataValue("EUR/USD",
                        1.2345m,
                        1.2346m))
            .GetRight();

        var sut = new ContributionService(this.surrealDbClient, this.validationService);

        // Act
        var result = await sut.CreateContribution(marketDataContribution,
            CancellationToken.None);

        // Assert
        result.Match(_ => throw new Exception("Should not be a contribution"),
            e => e.Value.Should().BeOfType<DatabaseError>());
    }

    [Theory]
    [InlineData("FxQuote", "EUR/USD", -1.2345, -1.2346)]
    public async Task
        CreateContribution_GivenInvalidMarketDataContribution_ShouldReturnValidationError(
            string marketDataType,
            string currencyPair,
            decimal bid,
            decimal ask)
    {
        // Arrange
        this.surrealDbClient.CreateRecord(Arg.Any<MarketDataContribution>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(GenerateCreateMarketDataContributionResponse(marketDataType,
                currencyPair,
                bid,
                ask)));

        this.surrealDbClient.UpdateRecord(Arg.Any<MarketDataContribution>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(GenerateCreateMarketDataContributionResponse(marketDataType,
                currencyPair,
                bid,
                ask)));

        this.validationService.Validate(Arg.Any<MarketDataContribution>(), Arg.Any<CancellationToken>())
            .Returns((ApplicationError)new ValidationServiceFail("Invalid Market Data Contribution",
                ValidationFailureType.InvalidFormat,
                new Dictionary<string, string>
                {
                    { "bid", "Negative Value" },
                    { "ask", "Negative Value" }
                }));

        var sut = new ContributionService(this.surrealDbClient,
            this.validationService);
        var marketDataContribution = MarketDataContribution.Create(marketDataType,
                new
                    MarketDataValue(currencyPair,
                        bid,
                        ask))
            .GetRight();

        // Act
        var result = await sut.CreateContribution(marketDataContribution,
            CancellationToken.None);

        // Assert
        result.Match(_ => throw new Exception("Should not be a contribution"),
            e => e.Value.Should().BeOfType<ValidationServiceFail>());
    }

    public static ApiResponse<MarketDataContribution> GenerateCreateMarketDataContributionResponse(
        string marketDataType,
        string currencyPair,
        decimal bid,
        decimal ask)
    {
        var marketDataContribution =
            MarketDataContribution.Create(marketDataType,
                    new MarketDataValue(currencyPair,
                        bid,
                        ask)
                )
                .GetRight();

        marketDataContribution.Id = Guid.NewGuid()
            .ToString();

        return GenerateSurrealCreatedApiResponse(marketDataContribution);
    }

    public static ApiResponse<T> GenerateSurrealCreatedApiResponse<T>(T createdRecord)
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
            new ApiResponse<T>(HttpStatusCode.Created)
                .SetQueryResponse(queryResponses);

        return record;
    }

    public static ApiResponse<T> GenerateErroredSurrealApiResponse<T>(T recordThatFailsToCreate)
    {
        var queryResponses = new List<QueryResponse<T>>
        {
            new(new List<T>(),
                "Unable to connect to DB",
                StatusResult.ERR)
        };

        var record =
            new ApiResponse<T>(HttpStatusCode.InternalServerError)
                .HasErrored("Unable to connect to DB",
                    new InvalidOperationException("Unable to connect to localhost:8999"))
                .SetQueryResponse(queryResponses);

        return record;
    }
}
