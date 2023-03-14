using MarketData.ContributionGatewayApi.Domain;
using OneOf;
using SurrealDb.Client;
using SurrealDb.Client.SurrealResponses;

namespace MarketData.ContributionGatewayApi.Application;

public class ContributionService : IContributionService
{
    private ISurrealDbClient SurrealDbClient { get; }

    public ContributionService( ISurrealDbClient surrealDbClient )
    {
        this.SurrealDbClient = surrealDbClient;
    }

    public async Task<OneOf<MarketDataContribution, ValidationError, DatabaseError>> CreateContribution(
        MarketDataContribution domainModel,
        CancellationToken cancellationToken )
    {
        // create
        var result = await this.SurrealDbClient.CreateRecord( domainModel,
                                                              cancellationToken );
        
        var databaseStatus = result.SerialiseResult?.First( )
                                  .Status;

        if ( databaseStatus is StatusResult.ERR )
        {
            return new 
                DatabaseError( "Database returned error status code" );
        }

        var createdRecord = result.SerialiseResult?.First( )
                                  .Result.First( );

        // validate against validation service
        
        return createdRecord;
    }
}
