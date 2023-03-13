using System.Text.Json;
using System.Text.Json.Serialization;
using SurrealDb.Client.SurrealResponses;

namespace SurrealDb.Client;

public class StatusResultConverter : JsonConverter<StatusResult>
{
    public override StatusResult Read( ref Utf8JsonReader reader,
                                       Type typeToConvert,
                                       JsonSerializerOptions options )
        => reader.TokenType switch
           {
               JsonTokenType.String => reader.GetString( ) switch
                                       {
                                           { } value => value switch
                                                        {
                                                            "OK"  => StatusResult.OK,
                                                            "ERR" => StatusResult.ERR,
                                                            _ => throw new
                                                                     JsonException( $"Could not understand {value}" )
                                                        },
                                           _ => StatusResult.ERR
                                       },
               _ => throw new JsonException( "Incorrect JSON token for StatusResult" )
           };

    public override void Write( Utf8JsonWriter writer,
                                StatusResult value,
                                JsonSerializerOptions options )
    {
        throw new NotImplementedException( );
    }
}
