namespace SurrealDb.Client;

public class DataNotSerialisedException : Exception
{
    public DataNotSerialisedException( string message ) : base( message ) { }
}