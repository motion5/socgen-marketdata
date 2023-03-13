using System.Net;
using SurrealDb.Client.SurrealResponses;

namespace SurrealDb.Client.Tests.Integration;

public class SurrealHttpClientTests : IClassFixture<SurrealDockerContainer>
{
    private sealed record Tester( string Id,
                                  string Name );

    private readonly SurrealDockerContainer fixture;

    public SurrealHttpClientTests( SurrealDockerContainer dockerContainer )
    {
        this.fixture = dockerContainer;
    }

    [Fact]
    public async Task GivenATable_WhenCreatingATable_ShouldBeSuccessful( )
    {
        // Arrange
        var client = this.SetupClient( );

        // Act
        var result = await client.CreateRecord( new Tester( string.Empty,
                                                            "Thing" ) );

        // Assert
        result.SerialiseResult?
              .First( )
              .Status
              .Should( )
              .Be( StatusResult.OK );
    }

    [Fact]
    public async Task GivenARecordId_WhenGettingARecordById_ShouldBeSuccessful( )
    {
        // Arrange
        var client = this.SetupClient( );

        var input = new Tester( string.Empty,
                                "New Thing" );

        // Act
        var result = await client.CreateRecord( input );
        var recordId = result.SerialiseResult!.First( )
                             .Result.First( )
                             .Id;
        var final = await client.GetRecord<Tester>( recordId );

        // Assert
        result.SerialiseResult?
              .First( )
              .Status
              .Should( )
              .Be( StatusResult.OK );

        var apiResult = final.SerialiseResult
                            ?.First( )
                             .Result.First( );
        apiResult?.Name
                  .Should( )
                  .Be( input.Name );

        apiResult?.Id.Should( )
                  .Be( recordId );
    }

    [Fact]
    public async Task GivenMultipleRecords_WhenGettingAllRecords_ShouldBeSuccessful( )
    {
        var client = SetupClient( );

        var input = new Tester( string.Empty,
                                "New Thing" );

        var result1 = await client.CreateRecord( input );

        result1.SerialiseResult?
               .First( )
               .Status
               .Should( )
               .Be( StatusResult.OK );
        var result2 = await client.CreateRecord( input );

        result2.SerialiseResult?
               .First( )
               .Status
               .Should( )
               .Be( StatusResult.OK );
        var result3 = await client.CreateRecord( input );

        result3.SerialiseResult?
               .First( )
               .Status
               .Should( )
               .Be( StatusResult.OK );

        // ACT

        var final = await client.SelectAll<Tester>( );

        final.ApiDetails
             .StatusCode
             .Should( )
             .Be( HttpStatusCode.OK );

        final.SerialiseResult!
             .First( )
             .Status.Should( )
             .Be( StatusResult.OK );

        final.SerialiseResult!
             .First( )
             .Result
             .Count
             .Should( )
             .BeGreaterThanOrEqualTo( 3,
                                      "we put in 3 result, but the database can have more due to other tests" );
    }

    [Fact]
    public async Task GivenATable_WhenDeletingAllRecords_ShouldBeSuccessful( )
    {
        // Arrange
        var client = SetupClient( );

        // Act
        var final = await client.DeleteAll<Tester>( );

        // Assert
        final.ApiDetails
             .StatusCode
             .Should( )
             .Be( HttpStatusCode.OK );

        final.SerialiseResult!
             .First( )
             .Status.Should( )
             .Be( StatusResult.OK );
    }

    [Fact]
    public async Task GivenARecordId_WhenDeletingARecordById_ShouldBeSuccessful( )
    {
        // Arrange
        var client = SetupClient( );

        var input = new Tester( string.Empty,
                                "New Thing" );


        // Act
        var result = await client.CreateRecord( input );

        result.SerialiseResult?
              .First( )
              .Status
              .Should( )
              .Be( StatusResult.OK );

        var recordId = result.SerialiseResult!.First( )
                             .Result.First( )
                             .Id;

        var getByIdResult = await client.GetRecord<Tester>( recordId );

        // Assert
        var apiResult = getByIdResult
                       .SerialiseResult
                      ?.First( )
                       .Result
                       .First( );

        apiResult?.Name
                  .Should( )
                  .Be( input.Name );

        // need to get the saved record id back
        apiResult?.Id.Should( )
                  .Be( recordId );

        // ACT
        var idToDelete = getByIdResult.SerialiseResult!.First( )
                                      .Result.First( )
                                      .Id;

        var finalResult = await client.DeleteRecord<Tester>( idToDelete );

        finalResult.ApiDetails
                   .StatusCode
                   .Should( )
                   .Be( HttpStatusCode.OK );

        finalResult.SerialiseResult!
                   .First( )
                   .Status.Should( )
                   .Be( StatusResult.OK );
    }

    private SurrealDbClient SetupClient( string database = "Test",
                                         string @namespace = "test" )
        => this.fixture.CreateClient( database,
                                      @namespace );
}
