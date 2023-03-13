using MarketData.ContributionGatewayApi;
using MarketData.ContributionGatewayApi.Infrastructure;
using SurrealDb.Client;

var builder = WebApplication.CreateBuilder( args );

builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen( );
builder.Services.AddHttpClient( );
builder.Services.AddJsonConverters( );
builder.Services.AddSurrealClient( );

var app = builder.Build( );

app.MapPost( "/contribution",
             async ( ContributionRequest request,
                     ISurrealDbClient dbClient,
                     CancellationToken cancellationToken ) =>
             {
                 try
                 {
                     var domainModel = request.ToDomainModel( );
                     // save to database
                     var result = await dbClient.CreateRecord( domainModel,
                                                               cancellationToken );

                     var createdRecord = result.SerialiseResult.First( )
                                               .Result.First( );

                     return Results.Created( $"contribution/{createdRecord.Id}",
                                             createdRecord );
                 }
                 catch ( Exception e ) when ( e is ArgumentNullException ||
                                              e is ArgumentException )
                 {
                     return Results.BadRequest( e.Message );
                 }
             } );

app.Run( );


public partial class Program { }
