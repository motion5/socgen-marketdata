using LanguageExt;
using MarketData.ContributionGatewayApi.Domain;

namespace MarketData.ContributionGatewayApi.Tests.Unit;

public static class EitherExtensions
{
    public static T GetRight<T>( this Either<ValidationError, T> either )
    {
        return either.Match( right => right,
                             _ => throw new Exception( "Expected Right" )
                           );
    }
}
