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
    public const string EntriesTableName = "PostsContextEntries";



    public async Task<(bool success, PostsContextObject.Raw postsContext)> Install_Async(
                IDbConnection dbConnection,
                TermObject.Raw sampleTerm ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                Name VARCHAR(256) NOT NULL,
                Description MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci
            );"
                //  CONSTRAINT FK_{TableName}_SimpleUserId FOREIGN KEY (SimpleUserId)
                //     REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id)
        );
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {EntriesTableName} (
                PostsContextId BIGINT NOT NULL,
                TermId BIGINT NOT NULL,
                Priority DOUBLE NOT NULL,
                IsRequired BOOLEAN NOT NULL,
                 PRIMARY KEY (PostsContextId, TermId),
                 CONSTRAINT FK_{EntriesTableName}_PostsContextId FOREIGN KEY (PostsContextId)
                    REFERENCES {TableName}(Id),
                 CONSTRAINT FK_{EntriesTableName}_TermId FOREIGN KEY (TermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id)
            );"
        );

        return await this.InstallSamples_Async( dbConnection, sampleTerm );
    }

    

    private readonly ILogger<ServerDataAccess_PostsContexts> Logger = logger;



    public async Task<PostsContextObject.Raw?> GetById_Async(
                IDbConnection dbCon,
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
            raw.Entries = (await dbCon.QueryAsync<PostsContextTermEntryObject.Raw>(
                $@"SELECT MyCtxEntries.PostsContextId, MyCtxEntries.TermId, MyCtxEntries.Priority, MyCtxEntries.IsRequired
                    FROM {EntriesTableName} AS MyCtxEntries
                    WHERE MyCtxEntries.PostsContextId = @PostsContextId;",
                new { PostsContextId = (long)postsContextId }
            )).ToArray();
        }

        return raw;
    }


    public async Task<IEnumerable<PostsContextObject.Raw>> GetByCriteria_Async(
                IDbConnection dbCon,
                ClientDataAccess_PostsContext.GetForCurrentUserByCriteria_Params parameters,
                bool alsoGetEntries ) {
        if( parameters.Ids.Any(id => id == 0) ) {
            throw new ArgumentException( "Some PostsContextIds are not valid (must be non-zero)." );
        }

        string sql1 = $"SELECT * FROM {TableName} AS MyContext WHERE";
        var sqlParams1 = new Dictionary<string, object>();

        bool needsAnd = false;
        
        if( parameters.Ids.Length >= 2 ) {
            sql1 += " MyContext.Id IN @Ids;";
            sqlParams1.Add( "@Ids", parameters.Ids );
            needsAnd = true;
        } else if( parameters.Ids.Length == 1 ) {
            sql1 += " MyContext.Id = @Id;";
            sqlParams1.Add( "@Id", parameters.Ids[0] );
            needsAnd = true;
        }

        if( !string.IsNullOrEmpty(parameters.NameContains) ) {
            if( needsAnd ) {
                sql1 += " AND";
            }
            sql1 += " MyContext.Name LIKE @NamePattern;";   // TODO: Validate
            sqlParams1.Add( "@NamePattern", "%"+parameters.NameContains+"%" );
            needsAnd = true;
        }

        IEnumerable<PostsContextObject.Raw> contexts
            = await dbCon.QueryAsync<PostsContextObject.Raw>(
                sql1,
                new DynamicParameters(sqlParams1)
            );

        if( alsoGetEntries ) {
            string sql2 = $@"SELECT MyCtxEntries.PostsContextId, MyCtxEntries.TermId, MyCtxEntries.Priority, MyCtxEntries.IsRequired
                FROM {EntriesTableName} AS MyCtxEntries
                WHERE MyCtxEntries.PostsContextId = @PostsContextId;";
            foreach( PostsContextObject.Raw ctx in contexts ) {
                var sqlParams2 = new Dictionary<string, object> { { "@PostsContextId", ctx.Id } };

                ctx.Entries = (await dbCon.QueryAsync<PostsContextTermEntryObject.Raw>(
                    sql2,
                    new DynamicParameters(sqlParams2)
                )).ToArray();
            }
        }

        return contexts;
    }


    public async Task<ClientDataAccess_PostsContext.CreateOrUpdate_Return> Create_Async(
                IDbConnection dbCon,
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

        string sqlInsertEntries = $@"INSERT INTO {EntriesTableName} (PostsContextId, TermId, Priority, IsRequired) 
                VALUES (@PostsContextId, @TermId, @Priority, @IsRequired);";
        foreach( PostsContextTermEntryObject.Raw entry in parameters.Entries ) {
            await dbCon.ExecuteAsync(
                sqlInsertEntries,
                new {
                    PostsContextId = postsContextId,
                    TermId = entry.TermId,
                    Priority = entry.Priority,
                    IsRequired = entry.IsRequired
                }
            );
        }

        return new ClientDataAccess_PostsContext.CreateOrUpdate_Return { Id = (PostsContextId)postsContextId };
    }


    public async Task<ClientDataAccess_PostsContext.CreateOrUpdate_Return> Update_Async(
                IDbConnection dbCon,
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
        
        int rowsAffected = await dbCon.ExecuteAsync(
            $@"DELETE FROM {EntriesTableName}
                WHERE PostsContextId = @PostsContextId;",
            new {
                PostsContextId = parameters.Id
            }
        );

        string sqlInsertEntries = $@"INSERT INTO {EntriesTableName}
            (PostsContextId, TermId, Priority, IsRequired) 
            VALUES (@PostsContextId, @TermId, @Priority, @IsRequired);";
        foreach( PostsContextTermEntryObject.Raw entry in parameters.Entries ) {
            await dbCon.ExecuteAsync(
                sqlInsertEntries,
                new {
                    PostsContextId = (long)parameters.Id,
                    TermId = (long)entry.TermId,
                    Priority = (long)entry.Priority,
                    IsRequired = entry.IsRequired
                }
            );
        }

        return new ClientDataAccess_PostsContext.CreateOrUpdate_Return {
            Id = parameters.Id ?? throw new InvalidOperationException("PostsContextObject.Prototype.Id cannot be null for update.")
        };
    }
}
