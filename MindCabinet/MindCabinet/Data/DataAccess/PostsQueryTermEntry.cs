using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_PostsQueryTermEntry( ILogger<ServerDataAccess_PostsQueryTermEntry> logger ) : IServerDataAccess {
    public const string TableName = "PostsQueryEntries";
    public const string TableColumn_PostsQueryId = "PostsQueryId";
    public const string TableColumn_TermId = "TermId";
    public const string TableColumn_Priority = "Priority";
    public const string TableColumn_IsRequired = "IsRequired";



    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {TableColumn_PostsQueryId} BIGINT NOT NULL,
                {TableColumn_TermId} BIGINT NOT NULL,
                {TableColumn_Priority} DOUBLE NOT NULL,
                {TableColumn_IsRequired} BOOLEAN NOT NULL,
                 PRIMARY KEY ({TableColumn_PostsQueryId}, {TableColumn_TermId}),
                 CONSTRAINT FK_{TableName}_{TableColumn_PostsQueryId} FOREIGN KEY ({TableColumn_PostsQueryId})
                    REFERENCES {ServerDataAccess_PostsQuery.TableName}({ServerDataAccess_PostsQuery.TableColumn_Id}),
                 CONSTRAINT FK_{TableName}_{TableColumn_TermId} FOREIGN KEY ({TableColumn_TermId})
                    REFERENCES {ServerDataAccess_Terms.TableName}({ServerDataAccess_Terms.TableColumn_Id})
            );"
        );

        return true;
    }

    

    private readonly ILogger<ServerDataAccess_PostsQueryTermEntry> Logger = logger;



    public async Task<PostsQueryTermEntryObject.Raw[]> GetByPostsQueryId_Async(
                IDbConnection dbCon,
                PostsQueryId postsQueryId ) {
        if( postsQueryId == 0 ) {
            throw new ArgumentException( "PostsQueryId is not valid (must be non-zero)." );
        }

        PostsQueryTermEntryObject.Raw[]? entries = (await dbCon.QueryAsync<PostsQueryTermEntryObject.Raw>(
            $@"SELECT
                    MyQueryEntries.{TableColumn_PostsQueryId},
                    MyQueryEntries.{TableColumn_TermId},
                    MyQueryEntries.{TableColumn_Priority},
                    MyQueryEntries.{TableColumn_IsRequired}
                FROM {TableName} AS MyQueryEntries
                WHERE MyQueryEntries.{TableColumn_PostsQueryId} = @PostsQueryId;",
            new { PostsQueryId = (long)postsQueryId }
        )).ToArray();

        return entries;
    }


    public async Task Create_Async(
                IDbConnection dbCon,
                PostsQueryId postsQueryId,
                PostsQueryTermEntryObject.Raw parameter ) {
        await dbCon.ExecuteAsync(
            $@"INSERT INTO {TableName} ({TableColumn_PostsQueryId}, {TableColumn_TermId}, {TableColumn_Priority}, {TableColumn_IsRequired}) 
                VALUES (@PostsQueryId, @TermId, @Priority, @IsRequired);",
            new {
                PostsQueryId = postsQueryId,
                TermId = parameter.TermId,
                Priority = parameter.Priority,
                IsRequired = parameter.IsRequired
            }
        );
    }
    
    public async Task<int> DeleteByPostsQueryId_Async(
                IDbConnection dbCon,
                PostsQueryId postsQueryId ) {
        if( postsQueryId == 0 ) {
            throw new ArgumentException( "PostsQuery Id is not valid (must be non-zero and non-null)." );
        }

        return await dbCon.ExecuteAsync(
            $@"DELETE FROM {TableName}
                WHERE {TableColumn_PostsQueryId} = @PostsQueryId;",
            new {
                PostsQueryId = postsQueryId
            }
        );
    }
}
