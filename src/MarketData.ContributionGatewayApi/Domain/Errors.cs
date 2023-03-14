namespace MarketData.ContributionGatewayApi.Domain;

public class ValidationError
{
    public ValidationError( IEnumerable<string> errors )
    {
        this.Errors = errors;     
    }

    public IEnumerable<string> Errors { get; }
}

public record DatabaseError( string Error );
