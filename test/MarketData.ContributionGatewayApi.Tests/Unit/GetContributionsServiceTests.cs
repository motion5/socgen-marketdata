using NSubstitute;
using SurrealDb.Client;
using Xunit;

namespace MarketData.ContributionGatewayApi.Tests.Unit;

public class CreateContributionServiceTests
{
    private readonly ISurrealDbClient surrealDbClient = Substitute.For<ISurrealDbClient>( );
 
    
}
