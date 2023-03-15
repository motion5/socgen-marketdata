using System.Collections;

namespace MarketData.ContributionGatewayApi.Domain;

public class MarketDataContributions : IEnumerable<MarketDataContribution>
{
    private readonly IEnumerable<MarketDataContribution> contributions;
    public MarketDataContributions( IEnumerable<MarketDataContribution> contributions )
    {
        this.contributions = contributions;
    }
    public IEnumerator<MarketDataContribution> GetEnumerator( ) => this.contributions.GetEnumerator( );
    IEnumerator IEnumerable.GetEnumerator( ) => this.GetEnumerator( );
}
