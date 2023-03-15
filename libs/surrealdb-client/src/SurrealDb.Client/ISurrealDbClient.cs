namespace SurrealDb.Client;

public interface ISurrealDbClient
{
    Task<ApiResponse<T>> Query<T>(
        string surrealQuery,
        CancellationToken? cancellationToken = null );

    Task<ApiResponse<T>> SelectAll<T>(
        CancellationToken? cancellationToken = null );

    Task<ApiResponse<T>> CreateRecord<T>(
        T record,
        CancellationToken? cancellationToken = null );

    Task<ApiResponse<T>> UpdateRecord<T>(
        T record,
        string id,
        CancellationToken? cancellationToken = null );

    Task<ApiResponse<T>> DeleteAll<T>(
        CancellationToken? cancellationToken = null );

    Task<ApiResponse<T>> DeleteRecord<T>(
        string id,
        CancellationToken? cancellationToken = null );

    Task<ApiResponse<T>> GetRecord<T>(
        string id,
        CancellationToken? cancellationToken = null );
}
