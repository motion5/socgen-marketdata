using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using MarketData.ContributionGatewayApi.Domain;
using MarketData.ContributionGatewayApi.Infrastructure;

namespace MarketData.ContributionGatewayApi;

public class MarketDataContributionConverter : JsonConverter<MarketDataContribution>
{
    public override MarketDataContribution? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options )
    {
        throw new NotImplementedException( );
    }

    public override void Write( Utf8JsonWriter writer,
                                MarketDataContribution value,
                                JsonSerializerOptions options )
    {
        writer.WriteStartObject( );
        writer.WriteString( nameof( value.Id ),
                            value.Id );
        writer.WriteString( nameof( value.MarketDataType ),
                            value.MarketDataType.ToString( ) );
        writer.WritePropertyName( nameof( value.MarketData ) );
        JsonSerializer.Serialize( writer,
                                  value.MarketData,
                                  options );
        writer.WriteString( nameof( value.Status ),
                            value.Status.ToString( ) );
        writer.WriteString( nameof( value.CreatedDate ),
                            value.CreatedDate.ToString( CultureInfo.InvariantCulture ) );
        writer.WriteEndObject( );
    }
}
