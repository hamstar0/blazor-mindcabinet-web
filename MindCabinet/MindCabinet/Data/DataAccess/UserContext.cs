using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserContexts( ILogger<ServerDataAccess_UserContexts> logger ) : IServerDataAccess {
    public const string TableName = "UserContexts";
    public const string EntriesTableName = "UserContextEntries";



    public async Task<(bool success, UserContextObject.Raw userContext)> Install_Async(
                IDbConnection dbConnection,
                TermObject.Raw sampleTerm,
                SimpleUserId defaultUserId ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                SimpleUserId BIGINT NOT NULL,
                Name VARCHAR(256) NOT NULL,
                Description MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
                 CONSTRAINT FK_{TableName}_SimpleUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id)
            );"
        );
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {EntriesTableName} (
                UserContextId BIGINT NOT NULL,
                TermId BIGINT NOT NULL,
                Priority DOUBLE NOT NULL,
                IsRequired BOOLEAN NOT NULL,
                 PRIMARY KEY (UserContextId, TermId),
                 CONSTRAINT FK_{EntriesTableName}_UserContextId FOREIGN KEY (UserContextId)
                    REFERENCES {TableName}(Id),
                 CONSTRAINT FK_{EntriesTableName}_TermId FOREIGN KEY (TermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id)
            );"
        );

        return await this.InstallSamples_Async( dbConnection, sampleTerm, defaultUserId );
    }

    

    private readonly ILogger<ServerDataAccess_UserContexts> Logger = logger;



    public async Task<UserContextObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                UserContextId userContextId,
                bool alsoGetEntries ) {
        var raw = await dbCon.QuerySingleOrDefaultAsync<UserContextObject.Raw>(
            $"SELECT * FROM {TableName} WHERE Id = @Id",
            new { Id = (long)userContextId }
        );
        if( raw is null ) {
            return null;
        }

        if( alsoGetEntries ) {
            raw.Entries = (await dbCon.QueryAsync<UserContextTermEntryObject.Raw>(
                $@"SELECT MyCtxEntries.UserContextId, MyCtxEntries.TermId, MyCtxEntries.Priority, MyCtxEntries.IsRequired
                    FROM {EntriesTableName} AS MyCtxEntries
                    WHERE MyCtxEntries.UserContextId = @UserContextId;",
                new { UserContextId = (long)userContextId }
            )).ToArray();
        }

        return raw;
    }


    public async Task<IEnumerable<UserContextObject.Raw>> GetByCriteria_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Params parameters,
                bool alsoGetEntries ) {
        string sql1 = $@"SELECT * FROM {TableName} AS MyContext
            WHERE MyContext.SimpleUserId = @UserId;";
        var sqlParams1 = new Dictionary<string, object> { { "@UserId", (long)simpleUserId } };

        if( parameters.Ids.Length >= 2 ) {
            sql1 += " AND MyContext.Id IN @Ids;";
            sqlParams1.Add( "@Ids", parameters.Ids );
        } else if( parameters.Ids.Length == 1 ) {
            sql1 += " AND MyContext.Id = @Id;";
            sqlParams1.Add( "@Id", parameters.Ids[0] );
        }

        if( parameters.NameContains is not null ) {
            sql1 += " AND MyContext.Name LIKE @NamePattern;";   // TODO: Validate
            sqlParams1.Add( "@NamePattern", "%"+parameters.NameContains+"%" );
        }

        IEnumerable<UserContextObject.Raw> contexts
            = await dbCon.QueryAsync<UserContextObject.Raw>(
                sql1,
                new DynamicParameters(sqlParams1)
            );

        if( alsoGetEntries ) {
            string sql2 = $@"SELECT MyCtxEntries.UserContextId, MyCtxEntries.TermId, MyCtxEntries.Priority, MyCtxEntries.IsRequired
                FROM {EntriesTableName} AS MyCtxEntries
                WHERE MyCtxEntries.UserContextId = @UserContextId;";
            foreach( UserContextObject.Raw ctx in contexts ) {
                var sqlParams2 = new Dictionary<string, object> { { "@UserContextId", ctx.Id } };

                ctx.Entries = (await dbCon.QueryAsync<UserContextTermEntryObject.Raw>(
                    sql2,
                    new DynamicParameters(sqlParams2)
                )).ToArray();
            }
        }

        return contexts;
    }


    public async Task<ClientDataAccess_UserContext.CreateForCurrentUser_Return> Create_Async(
                IDbConnection dbCon,
                SimpleUserId simpleUserId,
                UserContextObject.Raw parameters ) {
        long userContextId = await dbCon.ExecuteScalarAsync<long>(
            $@"INSERT INTO {TableName} (SimpleUserId, Name, Description) 
                VALUES (@SimpleUserId, @Name, @Description);
            SELECT LAST_INSERT_ID();",
            new {
                SimpleUserId = simpleUserId,
                Name = parameters.Name,
                Description = parameters.Description,
            }
        );

        string sqlInsertEntries = $@"INSERT INTO {EntriesTableName} (UserContextId, TermId, Priority, IsRequired) 
                VALUES (@UserContextId, @TermId, @Priority, @IsRequired);";
        foreach( UserContextTermEntryObject.Raw entry in parameters.Entries ) {
            await dbCon.ExecuteAsync(
                sqlInsertEntries,
                new {
                    UserContextId = userContextId,
                    TermId = entry.TermId,
                    Priority = entry.Priority,
                    IsRequired = entry.IsRequired
                }
            );
        }

        return new ClientDataAccess_UserContext.CreateForCurrentUser_Return( (UserContextId)userContextId );
    }


    public async Task<ClientDataAccess_UserContext.CreateForCurrentUser_Return> Update_Async(
                IDbConnection dbCon,
                UserContextObject.Raw parameters ) {
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
                WHERE UserContextId = @UserContextId;",
            new {
                UserContextId = parameters.Id
            }
        );
this.Logger.LogInformation( $"Deleted {rowsAffected} existing entries for UserContextId {parameters.Id}." );

        string sqlInsertEntries = $@"INSERT INTO {EntriesTableName}
            (UserContextId, TermId, Priority, IsRequired) 
            VALUES (@UserContextId, @TermId, @Priority, @IsRequired);";
        foreach( UserContextTermEntryObject.Raw entry in parameters.Entries ) {
            await dbCon.ExecuteAsync(
                sqlInsertEntries,
                new {
                    UserContextId = (long)parameters.Id,
                    TermId = (long)entry.TermId,
                    Priority = (long)entry.Priority,
                    IsRequired = entry.IsRequired
                }
            );
        }

        return new ClientDataAccess_UserContext.CreateForCurrentUser_Return( parameters.Id );
    }
}
