using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_ServerData : IServerDataAccess {
    public const string TableName = "ServerData";



    public async Task<bool> Install_Async(
                IDbConnection dbConnection,
                TermId userConceptTermId ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                UsersConceptTermId BIGINT NOT NULL,
                 CONSTRAINT FK_{TableName}_UsersConceptTermId FOREIGN KEY (UsersConceptTermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id)
            );"
        );

        await this.Create_Async( dbConnection, userConceptTermId );

        return true;
    }
    


    public async Task<ServerData.Raw> Get_Async( IDbConnection dbCon ) {
        ServerData.Raw? serverDataRaw = await dbCon.QuerySingleOrDefaultAsync<ServerData.Raw>(
            $"SELECT * FROM {TableName}",
            new { }
        );

        return serverDataRaw;
    }


    public async Task<ServerData.Raw> Create_Async(
                IDbConnection dbCon,
                TermId userConceptTermId ) {
        if( userConceptTermId == 0 ) {
            throw new ArgumentException( "TermId is not valid (must be non-zero)." );
        }

        try {
            long _ = await dbCon.ExecuteScalarAsync<long>(
                $@"INSERT INTO {TableName} (UsersConceptTermId) 
                    VALUES (@UsersConceptTermId);
                SELECT LAST_INSERT_ID();",
                new {
                    UsersConceptTermId = (long)userConceptTermId
                }
            );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException( $"Record could not be created (UsersConceptTermId: {userConceptTermId})", e );
        }

        return ServerData.CreateRaw(
            usersConceptTermId: userConceptTermId
        );
    }
}
