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


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    private async Task<bool> InstallSamples_Async(
                IDbConnection dbConnection,
                long defaultUserId,
                UserContextObject sampleContext ) {
        var sampleRaw = new UserAppDataObject.DatabaseEntry {
            SimpleUserId = defaultUserId,
            UserContextId = sampleContext.Id
        };

        await this.Create_Async(
            dbCon: dbConnection,
            simpleUserId: defaultUserId,
            userContext: sampleContext
        );

        return true;
    }
}
