using System.Text.Json;
using System.Text.Json.Serialization;
using SurrealDb.Client;

namespace MarketData.ContributionGatewayApi;

public static class JsonExtensions
{
    public static void PopulateJsonConverters( JsonSerializerOptions options )
    {
        options.Converters.Add( new StatusResultConverter( ) );
        options.Converters.Add( new JsonStringEnumConverter( ) );
    }

    public static IServiceCollection AddJsonConverters( this IServiceCollection services )
    {
        services.ConfigureHttpJsonOptions( options =>
                                           {
                                               options.SerializerOptions
                                                      .PropertyNameCaseInsensitive = true;
                                               options.SerializerOptions.PropertyNamingPolicy =
                                                   JsonNamingPolicy.CamelCase;
                                               PopulateJsonConverters( options.SerializerOptions );
                                           } );

        return services;
    }
}
