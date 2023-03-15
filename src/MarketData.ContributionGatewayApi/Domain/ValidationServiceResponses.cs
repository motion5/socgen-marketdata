namespace MarketData.ContributionGatewayApi.Domain;

public class ValidationServiceFail
{
    public ValidationServiceFail( string message,
                                  ValidationFailureType type,
                                  Dictionary<string, string> errors )
    {
        this.Message = message;
        this.Type = type;
        this.Errors = errors;
    }

    public string Message { get; }
    public ValidationFailureType Type { get; }
    public Dictionary<string, string> Errors { get; }
}

public enum ValidationFailureType
{
    InvalidFormat, 
    MIFID2Fail,
}

public class ValidationServiceSuccess { }
