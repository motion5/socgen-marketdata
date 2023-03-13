using System.Text;

namespace SurrealDb.Client;

public record BasicAuth( string Username,
                         string Password )
{
    public string ToBase64( )
        => Convert.ToBase64String( Encoding.UTF8.GetBytes( $"{Username}:{Password}" ) );
};
