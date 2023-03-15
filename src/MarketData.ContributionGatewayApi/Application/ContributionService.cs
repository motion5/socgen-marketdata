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

    public async Task<ContributionServiceResult>
        CreateContribution(
            MarketDataContribution contribution,
            CancellationToken cancellationToken )
    {
        // create
        var result = await this.SurrealDbClient.CreateRecord( contribution,
                                                              cancellationToken );

        if ( !string.IsNullOrWhiteSpace( result.Error ) )
        {
            return new
                DatabaseError( "Database returned error status code" );
        }

        var createdRecord = result.SerialiseResult?.First( )
                                  .Result.First( );

        // validate against validation service
        var validation = await this.ValidationService.Validate( contribution,
                                                                cancellationToken );

        return await validation
           .MatchAsync<ContributionServiceResult>( async success =>
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

                   var res=  updateResult.SerialiseResult?.First( )
                                      .Result.First( );

                   return res;
               },
               validationFail =>
               {
                   return new ValidationError( new List<string>( ) { validationFail.Message } );

               } );

    }
}
