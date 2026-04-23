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


public partial class ServerDataAccess_PostsContexts( ILogger<ServerDataAccess_PostsContexts> logger ) : IServerDataAccess {
    public const string TableName = "PostsContexts";



    public async Task<bool> Install_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                Name VARCHAR(256) NOT NULL,
                Description MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
            );"
                //  CONSTRAINT FK_{TableName}_SimpleUserId FOREIGN KEY (SimpleUserId)
                //     REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id)
        );

        return true;
    }

    

    private readonly ILogger<ServerDataAccess_PostsContexts> Logger = logger;



    public async Task<PostsContextObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                PostsContextId postsContextId,
                bool alsoGetEntries ) {
        if( postsContextId == 0 ) {
            throw new ArgumentException( "PostsContextId is not valid (must be non-zero)." );
        }

        var raw = await dbCon.QuerySingleOrDefaultAsync<PostsContextObject.Raw>(
            $"SELECT * FROM {TableName} WHERE Id = @Id",
            new { Id = (long)postsContextId }
        );
        if( raw is null ) {
            return null;
        }

        if( alsoGetEntries ) {
            raw.Entries = (await postsContextTermEntryData.GetByPostsContextId_Async(
                dbCon: dbCon,
                postsContextId: raw.Id
            )).ToArray();
        }

        return raw;
    }


    public async Task<IEnumerable<PostsContextObject.Raw>> GetByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                ClientDataAccess_PostsContext.GetForCurrentUserByCriteria_Params parameters,
                bool alsoGetEntries ) {
        if( parameters.Ids.Any(id => id == 0) ) {
            throw new ArgumentException( "Some PostsContextIds are not valid (must be non-zero)." );
        }

        string sql1 = $"SELECT * FROM {TableName} AS MyContext";
        var sqlParams1 = new Dictionary<string, object>();

        bool needsAnd = false;
        
        if( parameters.Ids.Length >= 2 ) {
            sql1 += " WHERE MyContext.Id IN @Ids";
            sqlParams1.Add( "@Ids", parameters.Ids );
            needsAnd = true;
        } else if( parameters.Ids.Length == 1 ) {
            sql1 += " WHERE MyContext.Id = @Id";
            sqlParams1.Add( "@Id", parameters.Ids[0] );
            needsAnd = true;
        }

        if( !string.IsNullOrEmpty(parameters.NameContains) ) {
            if( needsAnd ) {
                sql1 += " AND";
            } else {
                sql1 += " WHERE";
            }
            sql1 += " MyContext.Name LIKE @NamePattern";   // TODO: Validate
            sqlParams1.Add( "@NamePattern", "%"+parameters.NameContains+"%" );
            needsAnd = true;
        }
        sql1 += ";";

        IEnumerable<PostsContextObject.Raw> contexts
            = await dbCon.QueryAsync<PostsContextObject.Raw>(
                sql1,
                new DynamicParameters(sqlParams1)
            );

        if( alsoGetEntries ) {
            foreach( PostsContextObject.Raw ctx in contexts ) {
                ctx.Entries = (await postsContextTermEntryData.GetByPostsContextId_Async(
                    dbCon: dbCon,
                    postsContextId: ctx.Id
                )).ToArray();
            }
        }

        return contexts;
    }


    public async Task<ClientDataAccess_PostsContext.CreateOrUpdate_Return> Create_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                PostsContextObject.Prototype parameters ) {
        if( PostsContextObject.ValidateName(parameters.Name ?? "") ) {
            throw new ArgumentException( "PostsContext Name is not valid." );
        }

        long postsContextId = await dbCon.ExecuteScalarAsync<long>(
            $@"INSERT INTO {TableName} (Name, Description) 
                VALUES (@Name, @Description);
            SELECT LAST_INSERT_ID();",
            new {
                Name = parameters.Name,
                Description = parameters.Description,
            }
        );

        foreach( PostsContextTermEntryObject.Raw entry in parameters.Entries ) {
            await postsContextTermEntryData.Create_Async(
                dbCon: dbCon,
                postsContextId: (PostsContextId)postsContextId,
                parameter: entry
            );
        }

        return new ClientDataAccess_PostsContext.CreateOrUpdate_Return { Id = (PostsContextId)postsContextId };
    }


    public async Task<ClientDataAccess_PostsContext.CreateOrUpdate_Return> Update_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                PostsContextObject.Prototype parameters ) {
        if( parameters.Id == 0 || parameters.Id is null ) {
            throw new ArgumentException( "PostsContextObject.Prototype Id is not valid (must be non-zero and non-null)." );
        }
        if( PostsContextObject.ValidateName(parameters.Name ?? "") ) {
            throw new ArgumentException( "PostsContext Name is not valid." );
        }

        await dbCon.ExecuteAsync(
            $@"UPDATE {TableName}
                SET Name = @Name, Description = @Description
                WHERE Id = @Id;",
            new {
                Name = parameters.Name,
                Description = parameters.Description,
                Id = parameters.Id
            }
        );
        
        await postsContextTermEntryData.DeleteByPostsContextId_Async(
            dbCon: dbCon,
            postsContextId: parameters.Id.Value
        );

        foreach( PostsContextTermEntryObject.Raw entry in parameters.Entries ) {
            await postsContextTermEntryData.Create_Async(
                dbCon: dbCon,
                postsContextId: parameters.Id.Value,
                parameter: entry
            );
        }

        return new ClientDataAccess_PostsContext.CreateOrUpdate_Return {
            Id = parameters.Id
                ?? throw new InvalidOperationException("PostsContextObject.Prototype.Id cannot be null for update.")
        };
    }
}
