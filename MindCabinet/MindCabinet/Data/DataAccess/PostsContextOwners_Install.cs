using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.DataObjects;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using System.Data;
using System.Text;


namespace MindCabinet.Data.DataAccess;



public partial class ServerDataAccess_PostsContextOwners : IServerDataAccess {
    public const string TableName = "PostsContextOwners";

    public const string TableColumn_PostsContextId = "PostsContextId";

    public const string TableColumn_SimpleUserId = "SimpleUserId";

    // public const string TableColumn_IsAuthor = "isAuthor";

    public static readonly Dictionary<string, string> TableColumns = new() {
        { TableColumn_PostsContextId, "BIGINT NOT NULL" },
        { TableColumn_SimpleUserId, "BIGINT NOT NULL" },
        // { TableColumn_IsAuthor, "BOOLEAN NOT NULL" },
    };
    

    public async Task<bool> Install_Async(
                IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {string.Join(",\n    ", TableColumns.Select( c => $"{c.Key} {c.Value}" ))},
                 CONSTRAINT FK_{TableName}_{TableColumn_PostsContextId} FOREIGN KEY ({TableColumn_PostsContextId})
                    REFERENCES {ServerDataAccess_PostsContexts.TableName}({ServerDataAccess_PostsContexts.TableColumn_Id}),
                 CONSTRAINT FK_{TableName}_{TableColumn_SimpleUserId} FOREIGN KEY ({TableColumn_SimpleUserId})
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}({ServerDataAccess_SimpleUsers.TableColumn_Id}),
                 INDEX IDX_{TableColumn_PostsContextId} ({TableColumn_PostsContextId}),
                 INDEX IDX_{TableColumn_SimpleUserId} ({TableColumn_SimpleUserId})
            );"
            //  CONSTRAINT PK_{TableName}_CtxAndUserId PRIMARY KEY ({TableColumn_PostsContextId}, {TableColumn_SimpleUserId}),
            //    ON DELETE CASCADE
            //    ON UPDATE CASCADE
        );

        return true;
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
