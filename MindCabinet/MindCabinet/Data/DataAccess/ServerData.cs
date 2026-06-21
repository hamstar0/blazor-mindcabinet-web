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
using MindCabinet.Services;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_ServerData(
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    private static readonly SimpleCache<long, ServerDataObject.Raw> Cache_ById = new( refreshExpiryOnGet: true );



    private readonly StaticServerSettings ServerSettings = serverSettings;



    public async Task<ServerDataObject.Raw?> Get_Async( IDbConnection dbCon ) {
        if( ServerDataAccess_ServerData.Cache_ById.TryGet(0, out var cached) ) {
            return cached;
        }

        //

        ServerDataObject.Raw? serverDataRaw = await dbCon.QuerySingleOrDefaultAsync<ServerDataObject.Raw>(
            $"SELECT * FROM {TableName}",
            new { }
        );

        //

        if( serverDataRaw is not null ) {
            ServerDataAccess_ServerData.Cache_ById.Set(
                key: 0,
                value: serverDataRaw,
                expiry: this.ServerSettings.CacheExpirationDuration
            );
        }

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
                $@"INSERT INTO {TableName} ({TableColumn_UsersConceptTermId}) 
                    VALUES (@UsersConceptTermId);
                SELECT LAST_INSERT_ID();",
                new {
                    UsersConceptTermId = (long)usersConceptTermId
                }
            );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException( $"Record could not be created (UsersConceptTermId: {usersConceptTermId})", e );
        }

        var raw = ServerDataObject.CreateRaw(
            usersConceptTermId: usersConceptTermId
        );

        //

        ServerDataAccess_ServerData.Cache_ById.Set(
            key: 0,
            value: raw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        return raw;
    }
}
