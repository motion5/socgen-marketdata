using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using SurrealDb.Client.SurrealResponses;

namespace SurrealDb.Client;

public class SurrealDbClient : ISurrealDbClient
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions? serializerOptions;

    public SurrealDbClient(
        HttpClient httpClient,
        IOptions<SurrealClientOptions> surrealClientOptions,
        IOptions<JsonOptions> jsonOpts )
    {
        var options = surrealClientOptions.Value;
        client = httpClient;

        this.serializerOptions = jsonOpts.Value.SerializerOptions; 
         
        client.BaseAddress = new Uri(options.BaseAddress);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        client.DefaultRequestHeaders.Add("DB", options.Database);
        client.DefaultRequestHeaders.Add("NS", options.Namespace);

        var auth = new BasicAuth(options.Username, options.Password);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), auth.ToBase64());
    }

    public async Task<ApiResponse<T>> Query<T>(string surrealQuery,
        CancellationToken? cancellationToken = null)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, CreateUri("sql"));

        var stringContent = new StringContent(surrealQuery);
        requestMessage.Content = stringContent;

        return await TryRequest<T>(requestMessage,
            $"No data returned from sql query :: {surrealQuery}",
            cancellationToken);
    }

    public async Task<ApiResponse<T>> SelectAll<T>(
        CancellationToken? cancellationToken = null)
    {
        var table = typeof(T).Name;
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, CreateUri($"key/{table}"));

        return await TryRequest<T>(requestMessage,
            $"No data returned from selecting all data from :: {table}",
            cancellationToken);
    }


    public async Task<ApiResponse<T>> CreateRecord<T>(T record,
                                                      CancellationToken? cancellationToken = null)
    {
        var table = typeof(T).Name;
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, CreateUri($"key/{table}"));

        var jsoncontent = JsonSerializer.Serialize( record,
                                                    serializerOptions );
        var stringContent = new StringContent(jsoncontent, Encoding.UTF8,
            "application/json");

        requestMessage.Content = stringContent;

        return await TryRequest<T>(requestMessage,
            $"No data returned from creating record in :: {table}",
            cancellationToken);
    }

    public async Task<ApiResponse<T>> DeleteAll<T>(
        CancellationToken? cancellationToken = null)
    {
        var table = typeof(T).Name;
        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, CreateUri($"key/{table}"));

        return await TryRequest<T>(requestMessage,
            $"No data returned from deleting all records in :: {table}",
            cancellationToken);
    }

    public async Task<ApiResponse<T>> DeleteRecord<T>(string id,
                                                      CancellationToken? cancellationToken = null)
    {
        var table = typeof(T).Name;
        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, CreateUri($"key/{table}/{ExtractIdPart(id)}"));

        return await TryRequest<T>(requestMessage,
            $"No data returned from deleting record in :: {table} :: {id}",
            cancellationToken);
    }

    public async Task<ApiResponse<T>> GetRecord<T>(string id,
                                                   CancellationToken? cancellationToken = null)
    {
        var table = typeof(T).Name;
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, CreateUri($"key/{table}/{ExtractIdPart(id)}"));

        return await TryRequest<T>(requestMessage,
            $"No data returned from deleting record in :: {table} :: {id}",
            cancellationToken);
    }

    private async Task<ApiResponse<T>> TryRequest<T>(HttpRequestMessage requestMessage,
        string noDataMsg,
        CancellationToken? cancellationToken = null)
    {
        try
        {
            var response = await client.SendAsync(requestMessage,
                cancellationToken ?? CancellationToken.None);
            
            var apiResponse = new ApiResponse<T>(response.StatusCode);

            apiResponse = apiResponse.SetRequestDetails($"{requestMessage.Method} {requestMessage.RequestUri}",
                await (requestMessage.Content?.ReadAsStringAsync() ?? Task.FromResult(string.Empty)));

            var rawText = await response.Content.ReadAsStringAsync();
            var withRawText = apiResponse.SetRawText(rawText);

            (await response.Content.ReadAsStreamAsync()).Position = 0;

            try
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorRes = await response.Content
                        .ReadFromJsonAsync<ErrorResponse>(serializerOptions,
                            cancellationToken ??
                            CancellationToken.None);

                    return HandleNullErrResponse(errorRes,
                        noDataMsg,
                        withRawText);
                }


                var data = await response.Content
                    .ReadFromJsonAsync<IReadOnlyList<QueryResponse<T>>>(serializerOptions,
                        cancellationToken ??
                        CancellationToken
                            .None);

                return HandleNullData(data,
                    noDataMsg,
                    withRawText);
            }
            catch (JsonException jex)
            {
                return new ApiResponse<T>("Invalid JSON when deserializing Database response", jex);
            }
        }
        catch (InvalidOperationException oex)
        {
            return new ApiResponse<T>("Invalid Operation when making API request", oex);
        }
        catch (HttpRequestException hrex)
        {
            return new ApiResponse<T>("There was an exception when performing the request", hrex);
        }
        catch (TaskCanceledException tcex)
        {
            return new ApiResponse<T>("Task was cancelled while getting API response", tcex);
        }
        catch (UriFormatException ufex)
        {
            return new ApiResponse<T>("URI was invalid", ufex);
        }
    }

    private ApiResponse<T> HandleNullData<T>(IReadOnlyList<QueryResponse<T>>? data,
        string message,
        ApiResponse<T> currentResponse)
        => data switch
        {
            { } => currentResponse.SetQueryResponse(data),
            _ => currentResponse.HasErrored(message, new DataNotSerialisedException(message))
        };

    private ApiResponse<T> HandleNullErrResponse<T>(ErrorResponse? data,
        string message,
        ApiResponse<T> currentResponse)
        => data switch
        {
            { } => currentResponse.SetErrorResponse(data),
            _ => currentResponse.HasErrored(message, new DataNotSerialisedException(message))
        };

    private static Uri? CreateUri(string? uri) 
        => string.IsNullOrEmpty( uri ) 
               ? null 
               : new Uri( uri, UriKind.RelativeOrAbsolute );

    private static string ExtractIdPart(string id)
        => id.Contains(':')
            ? id.Split(":")[1]
            : id;
}
