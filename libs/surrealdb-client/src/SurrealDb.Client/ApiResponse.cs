using System.Net;
using SurrealDb.Client.SurrealResponses;

namespace SurrealDb.Client;

public record ApiDetails( string ResponseText,
                          HttpStatusCode StatusCode,
                          string RequestUri,
                          string RequestBody );

public class ApiResponse<T>
{
    public ApiResponse( )
    {
        ApiDetails = new ApiDetails( string.Empty,
                                     HttpStatusCode.SeeOther,
                                     string.Empty,
                                     string.Empty );
        IsSuccess = true;
    }
    
    public ApiResponse( HttpStatusCode statusCode)
    {
        ApiDetails = new ApiDetails( string.Empty,
                                     statusCode,
                                     string.Empty,
                                     string.Empty );
        IsSuccess = true;
    }

    public ApiResponse( string error,
                        Exception? exception )
    {
        IsSuccess = false;
        Error = error;
        Exception = exception;
    }

    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public Exception? Exception { get; private set; }

    public ApiResponse<T> HasErrored( string error,
                                      Exception? exception = null )
    {
        IsSuccess = false;
        Error = error;
        Exception = exception;
        return this;
    }


    public IReadOnlyList<QueryResponse<T>>? SerialiseResult { get; private set; }

    public ApiDetails ApiDetails { get; private set; }

    public ErrorResponse? ErrorResponse { get; private set; }

    public ApiResponse<T> SetQueryResponse( IReadOnlyList<QueryResponse<T>> serialiseResult )
    {
        SerialiseResult = serialiseResult;
        return this;
    }

    public ApiResponse<T> SetRawText( string responseText )
    {
        ApiDetails = ApiDetails with { ResponseText = responseText };
        return this;
    }

    public ApiResponse<T> SetStatusCode( HttpStatusCode statusCode )
    {
        ApiDetails = ApiDetails with { StatusCode = statusCode };
        return this;
    }

    public ApiResponse<T> SetRequestDetails( string requestUri,
                                             string requestBody = "" )
    {
        ApiDetails = ApiDetails with { RequestUri = requestUri, RequestBody = requestBody };
        return this;
    }

    public ApiResponse<T> SetErrorResponse( ErrorResponse response )
    {
        ErrorResponse = response;
        return this;
    }
}
