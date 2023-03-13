namespace SurrealDb.Client.SurrealResponses;

public enum StatusResult
{
    OK,
    ERR
}

public record BaseResponse( string Time,
                            StatusResult Status );

public record QueryResponse<T>( IReadOnlyList<T> Result, 
                                string Time,
                                StatusResult Status ) 
    : BaseResponse(Time, Status);

public record ErrorResponse( string Details,
                             string Description,
                             string Information,
                             int Code );
