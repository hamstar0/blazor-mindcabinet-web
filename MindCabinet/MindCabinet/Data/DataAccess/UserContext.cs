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


public partial class ServerDataAccess_UserContext {
    public const string TableName = "UserContexts";
    public const string EntriesTableName = "UserContextEntries";



    public async Task<bool> InstallUserContext_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE "+TableName+@" (
                ContextId BIGINT NOT NULL,
                SimpleUserId BIGINT NOT NULL,
                Name VARCHAR(256) NOT NULL,
                PRIMARY KEY (ContextId),
                CONSTRAINT FK_SessUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES "+ServerDataAccess_SimpleUsers.TableName+@"(Id)
            );"
        );
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE "+EntriesTableName+@" (
                ContextId BIGINT NOT NULL,
                TermId BIGINT NOT NULL,
                Priority DOUBLE NOT NULL,
                PRIMARY KEY (ContextId, TermId),
                CONSTRAINT FK_TermId FOREIGN KEY (TermId)
                    REFERENCES "+ServerDataAccess_Terms.TableName+@"(Id)
            );"
        );

        return true;
    }
    

    public async Task<ClientDataAccess_UserContext.Get_Return> GetByUserId_Async(
                IDbConnection dbCon,
                long simpleUserId ) {
        string sql1 = $"SELECT * FROM {TableName} AS MyContext"
            +" WHERE MyContext.SimpleUserId = @UserId;";
        var sqlParams1 = new Dictionary<string, object> { { "@UserId", simpleUserId } };

        IEnumerable<UserContextObject.UserContextWithTermEntries_DbData> contexts
            = await dbCon.QueryAsync<UserContextObject.UserContextWithTermEntries_DbData>(
                sql1,
                new DynamicParameters(sqlParams1)
            );

        foreach( UserContextObject.UserContextWithTermEntries_DbData ctx in contexts ) {
            string sql2 = @"SELECT MyContextEntries.TermId, MyContextEntries.Priority
                FROM {EntriesTableName} AS MyContextEntries
                WHERE MyContextEntries.ContextId = @ContextId;";
            var sqlParams2 = new Dictionary<string, object> { { "@ContextId", ctx.ContextId } };

            ctx.Entries = await dbCon.QueryAsync<UserContextObject.UserContextWithTermEntries_DbData.UserContextEntryDbData>(
                sql2, new DynamicParameters(sqlParams2) );
        }

        return new ClientDataAccess_UserContext.Get_Return( contexts );
    }


    public async Task<ClientDataAccess_UserContext.CreateForCurrentUser_Return> Create_Async(
                IDbConnection dbCon,
                long simpleUserId,
                ClientDataAccess_UserContext.CreateForCurrentUser_Params parameters ) {
        long userContextId = await dbCon.ExecuteScalarAsync<long>(
            @"INSERT INTO "+TableName+@" (SimpleUserId, Name) 
                VALUES (@SimpleUserId, @Name);
            SELECT LAST_INSERT_ID();",
            new {
                SimpleUserId = simpleUserId,
                Name = parameters.Name,
            }
        );

        string sqlInsertEntries = @"INSERT INTO "+EntriesTableName+@" (ContextId, TermId, Priority) 
                VALUES (@ContextId, @TermId, @Priority)";
        foreach( var entry in parameters.Entries ) {
            await dbCon.ExecuteAsync(
                sqlInsertEntries,
                new {
                    ContextId = userContextId,
                    TermId = entry.Term.Id,
                    Priority = entry.Priority,
                }
            );
        }

        return new ClientDataAccess_UserContext.CreateForCurrentUser_Return( userContextId );
    }
}
