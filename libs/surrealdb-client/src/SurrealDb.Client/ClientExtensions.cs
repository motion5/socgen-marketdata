using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SurrealDb.Client;

public static class ClientExtensions
{
    public static void AddSurrealClient( this IServiceCollection services,
                                         string configurationSection =
                                             SurrealClientOptions.SurrealClient )
    {
        services.AddOptions<SurrealClientOptions>( )
                .Configure<IConfiguration>( ( settings,
                                              configuration ) =>
                                            {
                                                configuration
                                                   .GetSection( configurationSection )
                                                   .Bind( settings );
                                            } );

        services.AddHttpClient<ISurrealDbClient, SurrealDbClient>( );
    }
}
