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


public partial class ServerDataAccess_PostsContextTermEntry( ILogger<ServerDataAccess_PostsContextTermEntry> logger ) : IServerDataAccess {
    public const string TableName = "PostsContextEntries";



    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                PostsContextId BIGINT NOT NULL,
                TermId BIGINT NOT NULL,
                Priority DOUBLE NOT NULL,
                IsRequired BOOLEAN NOT NULL,
                 PRIMARY KEY (PostsContextId, TermId),
                 CONSTRAINT FK_{TableName}_PostsContextId FOREIGN KEY (PostsContextId)
                    REFERENCES {ServerDataAccess_PostsContexts.TableName}(Id),
                 CONSTRAINT FK_{TableName}_TermId FOREIGN KEY (TermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id)
            );"
        );

        return true;
    }

    

    private readonly ILogger<ServerDataAccess_PostsContextTermEntry> Logger = logger;



    public async Task<PostsContextTermEntryObject.Raw[]> GetByPostsContextId_Async(
                IDbConnection dbCon,
                PostsContextId postsContextId ) {
        if( postsContextId == 0 ) {
            throw new ArgumentException( "PostsContextId is not valid (must be non-zero)." );
        }

        PostsContextTermEntryObject.Raw[]? entries = (await dbCon.QueryAsync<PostsContextTermEntryObject.Raw>(
            $@"SELECT MyCtxEntries.PostsContextId, MyCtxEntries.TermId, MyCtxEntries.Priority, MyCtxEntries.IsRequired
                FROM {TableName} AS MyCtxEntries
                WHERE MyCtxEntries.PostsContextId = @PostsContextId;",
            new { PostsContextId = (long)postsContextId }
        )).ToArray();

        return entries;
    }


    public async Task Create_Async(
                IDbConnection dbCon,
                PostsContextId postsContextId,
                PostsContextTermEntryObject.Raw parameter ) {
        await dbCon.ExecuteAsync(
            $@"INSERT INTO {TableName} (PostsContextId, TermId, Priority, IsRequired) 
                VALUES (@PostsContextId, @TermId, @Priority, @IsRequired);",
            new {
                PostsContextId = postsContextId,
                TermId = parameter.TermId,
                Priority = parameter.Priority,
                IsRequired = parameter.IsRequired
            }
        );
    }
    
    public async Task<int> DeleteByPostsContextId_Async(
                IDbConnection dbCon,
                PostsContextId postsContextId ) {
        if( postsContextId == 0 ) {
            throw new ArgumentException( "PostsContext Id is not valid (must be non-zero and non-null)." );
        }

        return await dbCon.ExecuteAsync(
            $@"DELETE FROM {TableName}
                WHERE PostsContextId = @PostsContextId;",
            new {
                PostsContextId = postsContextId
            }
        );
    }
}
