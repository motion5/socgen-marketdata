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
builder.Services.AddTransient<IValidationService, MockValidationService>( );

var app = builder.Build( );

app.MapGet( "/contribution",
            async ( IContributionService service,
              CancellationToken cancellationToken ) =>
            {
                var res = await service.GetContributions( cancellationToken );
                return res.Match( Results.Ok,
                           dbError => Results.Problem( "Unable to connect to Db" ) );
            } );

app.MapPost( "/contribution",
             async ( ContributionRequest request,
                     IContributionService service,
                     CancellationToken cancellationToken )
                 => await request
                         .ToMarketDataContribution( )
                         .ToAsync( )
                         .Map( async marketDataContribution =>
                               {
                                   var created =
                                       await service
                                          .CreateContribution( marketDataContribution,
                                                               cancellationToken );

                                   return created.Match( record
                                                             => Results
                                                                .Created( $"contribution/{record.Id}",
                                                                          record ),
                                                         Results.BadRequest,
                                                         dbError
                                                             => Results
                                                                .Problem( "Unable to connect to Db" ) );
                               } )
                         .Match( result => result.Result,
                                 Results.BadRequest ) );

app.Run( );


public partial class Program { }
