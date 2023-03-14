using MarketData.ContributionGatewayApi;
using MarketData.ContributionGatewayApi.Application;
using MarketData.ContributionGatewayApi.Infrastructure;
using SurrealDb.Client;

var builder = WebApplication.CreateBuilder( args );

builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen( );
builder.Services.AddHttpClient( );
builder.Services.AddJsonConverters( );
builder.Services.AddSurrealClient( );
builder.Services.AddTransient<IContributionService, ContributionService>( );

var app = builder.Build( );

app.MapPost( "/contribution",
             async ( ContributionRequest request,
                     IContributionService service,
                     CancellationToken cancellationToken ) =>
             {
                 try
                 {
                     var domainModel = request.ToMarketDataContribution( );

                     var created = await service.CreateContribution( domainModel,
                                                                     cancellationToken );

                     return created.Match( record => Results.Created( $"contribution/{record.Id}",
                                                                      record ),
                                           Results.BadRequest,
                                           dbError => Results.Problem( dbError.Error ) );
                 }
                 catch ( Exception e ) when ( e is ArgumentException or ArgumentNullException )
                 {
                     return Results.BadRequest( e.Message );
                 }
             } );

app.Run( );


public partial class Program { }
