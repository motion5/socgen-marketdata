using System.Text.Json;
using System.Text.Json.Serialization;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MarketData.ContributionGatewayApi.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using SurrealDb.Client;
using Xunit;

namespace MarketData.ContributionGatewayApi.Tests;

public class ContributionApiFactory : WebApplicationFactory<Program>,
                                      IAsyncLifetime
{
    private readonly DockerContainer surrealDbContainer;

    private const int Port = 8999;

    private static readonly BasicAuth Auth = new(Username: "root",
                                                 Password: "root");

    private readonly ISurrealDbClient surrealDbClient;
    internal readonly IValidationService ValidationService = Substitute.For<IValidationService>( );

    // resuse httpclients message handler to prevent port exhaustion and unnecessary connection creations
    private static readonly HttpMessageHandler Handler = new SocketsHttpHandler( );

    public ContributionApiFactory( )
    {
        // if env variable CI is set, do not setup docker container
        if ( IsCi( ) )
        {
            return;
        }

        var containerParameters =
            new List<string>
            {
                "start", "--log", "debug", "--user", "root", "--pass", "root", "memory"
            };

        // note: TestContainersBuilder is going through some refactorings causing obsolete warning
        //       this is a temporary fix to suppress the warning
#pragma warning disable 618
        this.surrealDbContainer = new TestcontainersBuilder<DockerContainer>( )
                                 .WithImage( "surrealdb/surrealdb:latest" )
                                 .WithCreateParameterModifier(
                                                              parameters
                                                                  => parameters.Cmd =
                                                                         containerParameters
                                                             )
                                 .WithPortBinding( Port,
                                                   8000 )
                                 .Build( );
#pragma warning restore 618


        // need to configure surreal client after to ensure json converters are added
        this.surrealDbClient = this.CreateDbClient( );
    }

    protected override void ConfigureWebHost( IWebHostBuilder builder )
    {
        builder
           .UseEnvironment( "Development" );
        builder
           .ConfigureTestServices( services
                                       =>
                                   {
                                       services.AddJsonConverters( );
                                       services
                                          .AddTransient
                                               <ISurrealDbClient>( _ => this.surrealDbClient );
                                       services
                                          .AddTransient<
                                               IValidationService>( _ => this
                                                                       .ValidationService );
                                   } );
    }

    public SurrealDbClient CreateDbClient( string database = "Test",
                                           string @namespace = "test" )
    {
        var jsonOptions = new JsonOptions( );
        jsonOptions.SerializerOptions.Converters.Add( new StatusResultConverter( ) );
        jsonOptions.SerializerOptions.Converters.Add( new JsonStringEnumConverter( ) );
        jsonOptions.SerializerOptions.PropertyNameCaseInsensitive = true;
        jsonOptions.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        return new(httpClient: new HttpClient( Handler ),
                   surrealClientOptions: Options.Create( new SurrealClientOptions
                                                         {
                                                             BaseAddress =
                                                                 $"http://localhost:{Port}",
                                                             Database = database,
                                                             Namespace = @namespace,
                                                             Username = Auth.Username,
                                                             Password = Auth.Password
                                                         } ),
                   Options.Create( jsonOptions ));
    }

    private Task CreateAsync( CancellationToken ct = default )
    {
        return IsCi( )
                   ? Task.CompletedTask
                   : this.surrealDbContainer.StartAsync( ct );
    }

    private Task DeleteAsync( CancellationToken ct = default )
    {
        return IsCi( )
                   ? Task.CompletedTask
                   : this.surrealDbContainer.StopAsync( ct );
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
