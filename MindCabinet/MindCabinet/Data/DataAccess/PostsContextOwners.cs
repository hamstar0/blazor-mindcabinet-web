using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.DataObjects;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;
using System.Data;
using System.Text;


namespace MindCabinet.Data.DataAccess;



public partial class ServerDataAccess_PostsContextOwners(
                ILogger<ServerDataAccess_PostsContextOwners> logger,
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    private static readonly SimpleCache<PostsContextId, PostsContextOwnersObject.Raw[]?> Cache_ByPCId = new( refreshExpiryOnGet: true );



    private readonly ILogger<ServerDataAccess_PostsContextOwners> Logger = logger;

    private readonly StaticServerSettings ServerSettings = serverSettings;



    public async Task<PostsContextOwnersObject.Raw[]> GetByPostsContextId_Async(
                IDbConnection dbCon,
                PostsContextId postsContextId ) {
        if( postsContextId == 0 ) {
            throw new ArgumentException( "PostsContextId is not valid." );
        }

        //

        if( ServerDataAccess_PostsContextOwners.Cache_ByPCId.TryGet(postsContextId, out var cached) ) {
            return cached ?? [];
        }

        //

        IEnumerable<PostsContextOwnersObject.Raw> raws = await dbCon.QueryAsync<PostsContextOwnersObject.Raw>(
            $"SELECT * FROM {TableName} WHERE {TableColumn_PostsContextId} = @Id",
            new { Id = (long)postsContextId }
        );
        if( raws is null ) {
            return [];
        }

        //

        var rawsArr = raws.ToArray();

        ServerDataAccess_PostsContextOwners.Cache_ByPCId.Set(
            key: postsContextId,
            value: rawsArr,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        return rawsArr;
    }
    

    public async Task<PostsContextOwnersObject.Raw[]> Create_Async(
                IDbConnection dbCon,
                PostsContextId postsContextId,
                SimpleUserId[] ownerUserIds ) {
        if( postsContextId == 0 ) {
            throw new ArgumentException( "PostsContextId is not valid." );
        }
        if( ownerUserIds.Length == 0 ) {
            throw new ArgumentException( "No SimpleUserIds." );
        }
        if( ownerUserIds.Any( id => id == 0 ) ) {
            throw new ArgumentException( "Some SimpleUserIds not valid." );
        }

        //
        
        var sqlBuilder = new StringBuilder(
            @$"INSERT INTO {TableName}
                ({TableColumn_PostsContextId}, {TableColumn_SimpleUserId})
            VALUES "
        );
        var sqlParams = new DynamicParameters();

        for( int i=0; i<ownerUserIds.Length; i++ ) {
            if( i > 0 ) {
                sqlBuilder.Append( ", " );
            }

            string postsContextIdName = $"@PostsContextId{i}";
            string simpleUserParamName = $"@SimpleUserId{i}";
            sqlBuilder.Append( $"({postsContextIdName}, {simpleUserParamName}, 0)" );

            sqlParams.Add( postsContextIdName, (long)postsContextId );
            sqlParams.Add( simpleUserParamName, (long)ownerUserIds[i] );
        }

        await dbCon.ExecuteAsync( sqlBuilder.ToString(), sqlParams );

        //

        return ownerUserIds
            .Select( ownerUserId => PostsContextOwnersObject.CreateRaw(
                postsContextId: postsContextId,
                simpleUserId: ownerUserId
            ) ).ToArray();
    }
}
