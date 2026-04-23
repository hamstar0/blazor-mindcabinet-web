using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;
using System.Data;
using MindCabinet.DataObjects;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_ServerData : IServerDataAccess {
    public const string TableName = "ServerData";



    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                UsersConceptTermId BIGINT NOT NULL,
                 CONSTRAINT FK_{TableName}_UsersConceptTermId FOREIGN KEY (UsersConceptTermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id)
            );"
        );

        return true;
    }

    public async Task<bool> Install_After_Async(
                IDbConnection dbConnection,
                TermId usersConceptTermId ) {
        return await this.InstallSamples_Async( dbConnection, usersConceptTermId );
    }
    


    public async Task<ServerDataObject.Raw?> Get_Async( IDbConnection dbCon ) {
        ServerDataObject.Raw? serverDataRaw = await dbCon.QuerySingleOrDefaultAsync<ServerDataObject.Raw>(
            $"SELECT * FROM {TableName}",
            new { }
        );

        return serverDataRaw;
    }


    private async Task<ServerDataObject.Raw> Create_Async(
                IDbConnection dbCon,
                TermId usersConceptTermId ) {
        if( usersConceptTermId == 0 ) {
            throw new ArgumentException( "UsersConceptTermId is not valid (must be non-zero)." );
        }

        try {
            long _ = await dbCon.ExecuteScalarAsync<long>(
                $@"INSERT INTO {TableName} (UsersConceptTermId) 
                    VALUES (@UsersConceptTermId);
                SELECT LAST_INSERT_ID();",
                new {
                    UsersConceptTermId = (long)usersConceptTermId
                }
            );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException( $"Record could not be created (UsersConceptTermId: {usersConceptTermId})", e );
        }

        return ServerDataObject.CreateRaw(
            usersConceptTermId: usersConceptTermId
        );
    }
}
