using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data;


public partial class ServerDbAccess {
    public async Task<bool> InstallUserContext_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE UserContexts (
                ContextId BIGINT NOT NULL,
                SimpleUserId BIGINT NOT NULL,
                Name VARCHAR(256) NOT NULL,
                PRIMARY KEY (ContextId),
                CONSTRAINT FK_SessUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES SimpleUsers(Id)
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE UserContextEntries (
                ContextId BIGINT NOT NULL,
                TermId BIGINT NOT NULL,
                Priority DOUBLE NOT NULL,
                PRIMARY KEY (ContextId, TermId),
                CONSTRAINT FK_TermId FOREIGN KEY (TermId)
                    REFERENCES Terms(Id)
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        return true;
    }
    

    public async Task<IEnumerable<long>> GetUserContextsByUserId_Async(
                IDbConnection dbCon,
                ClientDbAccess.GetSimpleUserFavoriteTagIdsParams parameters ) {
	}


    public async Task AddUserContext_Async(
                IDbConnection dbCon,
                ClientDbAccess.AddSimpleUserFavoriteTagsByIdParams parameters ) {
    }
}
