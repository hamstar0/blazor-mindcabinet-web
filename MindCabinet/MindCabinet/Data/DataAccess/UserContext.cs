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


public partial class ServerDataAccess_UserContext : IServerDataAccess {
    public const string TableName = "UserContexts";
    public const string EntriesTableName = "UserContextEntries";



    public async Task<bool> Install_Async(
                IDbConnection dbConnection,
                TermObject sampleTerm,
                long defaultUserId ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                ContextId BIGINT NOT NULL,
                SimpleUserId BIGINT NOT NULL,
                Name VARCHAR(256) NOT NULL,
                PRIMARY KEY (ContextId),
                CONSTRAINT FK_UserContextEntries_SessUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id)
            );"
        );
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {EntriesTableName} (
                ContextId BIGINT NOT NULL,
                TermId BIGINT NOT NULL,
                Priority DOUBLE NOT NULL,
                IsRequired BOOLEAN NOT NULL,
                PRIMARY KEY (ContextId, TermId),
                CONSTRAINT FK_UserContextEntries_TermId FOREIGN KEY (TermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id)
            );"
        );

        return await this.InstallSamples_Async( dbConnection, sampleTerm, defaultUserId );
    }
    

    public async Task<UserContextObject?> GetById_Async( IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_Terms_Sets termSetsData,
                long contextId ) {
        UserContextObject.DatabaseEntry? usrCtxRaw = await dbCon.QuerySingleAsync<UserContextObject.DatabaseEntry?>(
            $"SELECT * FROM {TableName} WHERE ContextId = @ContextId",
            new { ContextId = contextId }
        );

        if( usrCtxRaw is null ) {
            return null;
        }

        return await usrCtxRaw.CreateUserContext_Async( async (ids) => {
            return await termsData.GetByIds_Async( dbCon, termsData, ids );
        } );
    }


    public async Task<ClientDataAccess_UserContext.Get_Return> GetByCriteria_Async(
                IDbConnection dbCon,
                long simpleUserId,
                ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Params parameters ) {
        string sql1 = $@"SELECT * FROM {TableName} AS MyContext
            WHERE MyContext.SimpleUserId = @UserId;";
        var sqlParams1 = new Dictionary<string, object> { { "@UserId", simpleUserId } };

        if( parameters.Ids.Count >= 2 ) {
            sql1 += " AND MyContext.ContextId IN @Ids;";
            sqlParams1.Add( "@Ids", parameters.Ids );
        } else if( parameters.Ids.Count == 1 ) {
            sql1 += " AND MyContext.ContextId = @Id;";
            sqlParams1.Add( "@Id", parameters.Ids[0] );
        }

        if( parameters.NameContains is not null ) {
            sql1 += " AND MyContext.Name LIKE @NamePattern;";   // TODO: Validate
            sqlParams1.Add( "@NamePattern", "%"+parameters.NameContains+"%" );
        }

        IEnumerable<UserContextObject.DatabaseEntry> contexts
            = await dbCon.QueryAsync<UserContextObject.DatabaseEntry>(
                sql1,
                new DynamicParameters(sqlParams1)
            );

        foreach( UserContextObject.DatabaseEntry ctx in contexts ) {
            string sql2 = $@"SELECT MyContextEntries.TermId, MyContextEntries.Priority, MyContextEntries.IsRequired
                FROM {EntriesTableName} AS MyContextEntries
                WHERE MyContextEntries.ContextId = @ContextId;";
            var sqlParams2 = new Dictionary<string, object> { { "@ContextId", ctx.ContextId } };

            ctx.Entries = await dbCon.QueryAsync<UserContextTermEntryObject.DatabaseEntry>(
                sql2, new DynamicParameters(sqlParams2) );
        }

        return new ClientDataAccess_UserContext.Get_Return( contexts.ToArray() );
    }


    public async Task<ClientDataAccess_UserContext.CreateForCurrentUser_Return> Create_Async(
                IDbConnection dbCon,
                long simpleUserId,
                UserContextObject.DatabaseEntry parameters ) {
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

        string sqlInsertEntries = $@"INSERT INTO {EntriesTableName} (ContextId, TermId, Priority, IsRequired) 
                VALUES (@ContextId, @TermId, @Priority, @IsRequired);";
        foreach( UserContextTermEntryObject.DatabaseEntry entry in parameters.Entries ) {
            await dbCon.ExecuteAsync(
                sqlInsertEntries,
                new {
                    ContextId = userContextId,
                    TermId = entry.TermId,
                    Priority = entry.Priority,
                    IsRequired = entry.IsRequired
                }
            );
        }

        return new ClientDataAccess_UserContext.CreateForCurrentUser_Return( userContextId );
    }


    public async Task<ClientDataAccess_UserContext.CreateForCurrentUser_Return> Update_Async(
                IDbConnection dbCon,
                long userContextId,
                UserContextObject.DatabaseEntry parameters ) {
        await dbCon.ExecuteAsync(
            $@"UPDATE {TableName}
                SET Name = @Name, Description = @Description
                WHERE ContextId = @ContextId;",
            new {
                Name = parameters.Name,
                Description = parameters.Description,
                ContextId = parameters.ContextId
            }
        );
        
        await dbCon.ExecuteAsync(
            $@"DELETE FROM {EntriesTableName}
                WHERE ContextId = @ContextId;",
            new {
                ContextId = parameters.ContextId
            }
        );

        string sqlInsertEntries = $@"INSERT INTO {EntriesTableName} (ContextId, TermId, Priority, IsRequired) 
            VALUES (@ContextId, @TermId, @Priority, @IsRequired);";
        foreach( UserContextTermEntryObject.DatabaseEntry entry in parameters.Entries ) {
            await dbCon.ExecuteAsync(
                sqlInsertEntries,
                new {
                    ContextId = userContextId,
                    TermId = entry.TermId,
                    Priority = entry.Priority,
                    IsRequired = entry.IsRequired
                }
            );
        }

        return new ClientDataAccess_UserContext.CreateForCurrentUser_Return( userContextId );
    }
}
