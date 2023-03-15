using MarketData.ContributionGatewayApi.Domain;
using SurrealDb.Client;

namespace MarketData.ContributionGatewayApi.Application;

public class ContributionService : IContributionService
{
    private ISurrealDbClient SurrealDbClient { get; }
    private IValidationService ValidationService { get; }

    public ContributionService( ISurrealDbClient surrealDbClient,
                                IValidationService validationService )
    {
        this.SurrealDbClient = surrealDbClient;
        this.ValidationService = validationService;
    }

    public async Task<CreateContributionResult>
        CreateContribution(
            MarketDataContribution contribution,
            CancellationToken cancellationToken )
    {
        var result = await this.SurrealDbClient.CreateRecord( contribution,
                                                              cancellationToken );

        if (!result.IsSuccess)
        {
            return new
                DatabaseError( "Database returned error status code" );
        }

        var createdRecord = result.SerialiseResult?.First( )
                                  .Result.First( );

        var validation = await this.ValidationService.Validate( contribution,
                                                                cancellationToken );

        return await validation
           .MatchAsync<CreateContributionResult>( async _ =>
               {
                   var recordToUpdate = createdRecord.SetStatus( MarketDataContributionStatus.Validated );
                   
                   // update record
                   var updateResult =
                       await this.SurrealDbClient.UpdateRecord( recordToUpdate,
                                                                recordToUpdate.Id,
                                                                cancellationToken );

                   if (!updateResult.IsSuccess)
                   {
                       return new
                           DatabaseError( "Database returned error status code" );
                   }

                   var res =  updateResult.SerialiseResult?.First( )
                                      .Result.First( );

                   return res;
               },
               validationFail => new ValidationError( new List<string> { validationFail.Message } ) );

    }

    public async Task<GetContributionsResult> GetContributions( CancellationToken cancellationToken )
    {
        var result = await this.SurrealDbClient.SelectAll<MarketDataContribution>( cancellationToken );
        
        if (!result.IsSuccess)
        {
            return new
                DatabaseError( "Database returned error status code" );
        }
        
        var res = result.SerialiseResult?.First( )
                     .Result; 
        
        return res.ToList( );
    }
}
