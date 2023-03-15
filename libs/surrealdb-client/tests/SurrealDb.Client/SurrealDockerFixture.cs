using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace SurrealDb.Client.Tests;

public class SurrealDockerContainer : IAsyncLifetime
{
    private readonly DockerContainer? surrealDbContainer;

    private const int Port = 8999;

    private static readonly BasicAuth Auth = new(Username: "root",
                                                 Password: "root");

    // resuse httpclients message handler to prevent port exhaustion and unnecessary connection creations
    private static readonly HttpMessageHandler Handler = new SocketsHttpHandler( );

    public SurrealDockerContainer( )
    {
        // if env variable CI is set, do not setup docker container
        if ( IsCi( ) )
        {
            return;
        }

        var surrealStartParams = new List<string>
                                 {
                                     "start",
                                     "--log",
                                     "debug",
                                     "--user",
                                     "root",
                                     "--pass",
                                     "root",
                                     "memory"
                                 };

#pragma warning disable 618
        this.surrealDbContainer =
            new TestcontainersBuilder<DockerContainer>( )
               .WithImage( "surrealdb/surrealdb:latest" )
               .WithCreateContainerParametersModifier( parameters
                                                           => parameters.Cmd = surrealStartParams )
               .WithPortBinding( Port,
                                 8000 )
               .Build( );
#pragma warning restore 618
    }

    public SurrealDbClient CreateClient( string database,
                                         string @namespace )
        => new(httpClient: new HttpClient( Handler ),
               surrealClientOptions:
               Options.Create( new SurrealClientOptions
                               {
                                   BaseAddress = $"http://localhost:{Port}",
                                   Database = database,
                                   Namespace = @namespace,
                                   Username = Auth.Username,
                                   Password = Auth.Password
                               } ),
               jsonOpts: Options.Create( new JsonOptions( ) ));

    private Task CreateAsync( CancellationToken ct = default )
    {
        return IsCi( )
                   ? Task.CompletedTask
                   : this.surrealDbContainer?.StartAsync( ct ) ??
                     throw new InvalidOperationException( "Surreal Db Container is null" );
    }

    private Task DeleteAsync( CancellationToken ct = default )
    {
        return IsCi( )
                   ? Task.CompletedTask
                   : this.surrealDbContainer?.StopAsync( ct ) ??
                     throw new InvalidOperationException( "Surreal Db Container is null" );
    }

    public Task InitializeAsync( )
    {
        return this.CreateAsync( );
    }

    public Task DisposeAsync( )
    {
        return this.DeleteAsync( );
    }

    private static bool IsCi( )
    {
        var ci = Environment.GetEnvironmentVariable( "CI" );
        return !string.IsNullOrWhiteSpace( ci ) && ci == "true";
    }
}
