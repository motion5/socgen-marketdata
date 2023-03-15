using OneOf;

namespace MarketData.ContributionGatewayApi.Domain;

public class ApplicationError :
    OneOfBase<ValidationError, DatabaseError, ValidationServiceFail>
{
    protected ApplicationError(
       OneOf<ValidationError, DatabaseError, ValidationServiceFail> input ) :
        base( input ) { }

    public static implicit operator ApplicationError( ValidationError fail )
        => new(fail);

    public static implicit operator ApplicationError( DatabaseError fail )
        => new(fail);
    
    public static implicit operator ApplicationError( ValidationServiceFail fail ) 
        => new(fail);
}
