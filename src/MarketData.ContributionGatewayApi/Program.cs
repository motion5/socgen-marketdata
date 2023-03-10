using MarketData.ContributionGatewayApi.Models;

var builder = WebApplication.CreateBuilder( args );
var app = builder.Build( );

app.MapGet( "/",
            ( ) => "Hello World!" );

app.MapPost( "/contribution",
             ( ContributionRequest request ) =>
             {
                 var response = new ContributionResponse(
                                                         Guid.NewGuid( ),
                                                         request.MarketDataType,
                                                         request.MarketData,
                                                         "Validated",
                                                         DateTime.UtcNow );

                 return Results.Created( $"/contribution/{response.Id}",
                                         response );
             } );

app.Run( );


public partial class Program { }
