using LanguageExt;
using MarketData.ContributionGatewayApi.Domain;
using SurrealDb.Client;

namespace MarketData.ContributionGatewayApi.Application;

public class ContributionService : IContributionService
{
    private ISurrealDbClient SurrealDbClient { get; }
    private IValidationService ValidationService { get; }

    public ContributionService(ISurrealDbClient surrealDbClient,
        IValidationService validationService)
    {
        this.SurrealDbClient = surrealDbClient;
        this.ValidationService = validationService;
    }

    public async Task<Either<ApplicationError, MarketDataContribution>>
        CreateContribution(
            MarketDataContribution contribution,
            CancellationToken cancellationToken) =>
        await from created in this.CreateContributionRecord(contribution, cancellationToken).ToAsync()
            from validatedRecord in this.ValidationService.Validate(created, cancellationToken).ToAsync().Map(_ => created)
            from updatedRecord in this.UpdateContributionRecord(validatedRecord, cancellationToken).ToAsync()
            select updatedRecord;


    public async Task<GetContributionsResult> GetContributions(CancellationToken cancellationToken)
    {
        var result = await this.SurrealDbClient.SelectAll<MarketDataContribution>(cancellationToken);

        if (!result.IsSuccess)
        {
            return new
                DatabaseError("Database returned error status code");
        }

        var res = result.SerialiseResult?.First()
            .Result;

        if (res is null)
        {
            return new DatabaseError("No records found");
        }

        return new MarketDataContributions(res);
    }

    private async Task<Either<ApplicationError, MarketDataContribution>>
        CreateContributionRecord(MarketDataContribution contribution,
            CancellationToken cancellationToken)
    {
        var result = await this.SurrealDbClient.CreateRecord(contribution,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return Either<ApplicationError, MarketDataContribution>
                .Left(new DatabaseError("Database returned error status code"));
        }

        var createdRecord = result.SerialiseResult?.First()
            .Result.First();

        if (createdRecord is null)
        {
            return Either<ApplicationError, MarketDataContribution>
                .Left(new DatabaseError("Unable to retrieve created record"));
        }

        return createdRecord;
    }

    private async Task<Either<ApplicationError, MarketDataContribution>>
        UpdateContributionRecord(MarketDataContribution contribution,
            CancellationToken cancellationToken)
    {
        var recordToUpdate = contribution.SetStatus(MarketDataContributionStatus.Validated);
        var recordId = recordToUpdate.Id ?? string.Empty;

        if (string.IsNullOrWhiteSpace(recordId))
        {
            return Either<ApplicationError, MarketDataContribution>
                .Left(new DatabaseError("Unrecoverable error. Contribution Id doesn't exist"));
        }

        var updateResult =
            await this.SurrealDbClient.UpdateRecord(recordToUpdate,
                recordId,
                cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return Either<ApplicationError, MarketDataContribution>
                .Left(new DatabaseError("Database returned error status code"));
        }

        var res = updateResult.SerialiseResult?.First()
            .Result.First();

        if (res is null)
        {
            return Either<ApplicationError, MarketDataContribution>
                .Left(new DatabaseError("Unable to retrieve updated record"));
        }

        return res;
    }
}
