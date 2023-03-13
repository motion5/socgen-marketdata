using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SurrealDb.Client.Tests;

public class ExtensionTests
{
    [Fact]
    public void GivenServiceCollection_WhenAddingSurrealClient_ShouldRegisterClientOptions( )
    {
        // Arrange
        var serviceCollection = new ServiceCollection( );

        // Act
        serviceCollection.AddSurrealClient( );

        // Assert
        var firstOrDefault =
            serviceCollection.SingleOrDefault( x => x.ServiceType ==
                                                    typeof( IOptions<SurrealClientOptions> )
                                                       .GetGenericTypeDefinition( ) );
        firstOrDefault.Should( )
                      .NotBeNull( );
    }
}
