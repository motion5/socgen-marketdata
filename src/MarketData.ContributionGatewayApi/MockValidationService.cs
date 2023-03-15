using LanguageExt;
using MarketData.ContributionGatewayApi.Application;
using MarketData.ContributionGatewayApi.Domain;

public class MockValidationService : IValidationService
{
    public async Task<Either<ValidationServiceFail, ValidationServiceSuccess>> Validate(
        MarketDataContribution contribution,
        CancellationToken cancellationToken )
    {
        if ( new Random( ).Next( 0,
                                 100 ) > 50 )
        {
            return await Task.FromResult( Either<ValidationServiceFail, ValidationServiceSuccess>
                                             .Left( new ValidationServiceFail( "Failed MIFID2",
                                                                               ValidationFailureType
                                                                                  .MIFID2Fail,
                                                                               new Dictionary<string
                                                                                 , string>
                                                                               {
                                                                                   {
                                                                                       "Field1",
                                                                                       "Error1"
                                                                                   }
                                                                               } ) ) );
        }

        return await Task.FromResult( Either<ValidationServiceFail, ValidationServiceSuccess>
                                         .Right( new ValidationServiceSuccess( ) ) );
    }
}
