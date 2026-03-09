using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserContexts : IServerDataAccess {
    private async Task<(bool success, UserContextObject.DatabaseEntry userContext)> InstallSamples_Async(
                IDbConnection dbConnection,
                TermObject.DatabaseEntry sampleTerm,
                long defaultUserId ) {
        var sampleRawEntry = new UserContextTermEntryObject.DatabaseEntry {
            TermId = sampleTerm.Id,
            Priority = 1.0,
            IsRequired = true
        };
        var sampleRawCtx = new UserContextObject.DatabaseEntry {
            Name = "Default Context",
            Description = "A sample user context.",
            Entries = [sampleRawEntry]
        };

        long usrCtxId = (await this.Create_Async(
            dbCon: dbConnection,
            simpleUserId: defaultUserId,
            parameters: sampleRawCtx
        )).UserContextId;

        sampleRawCtx.Id = usrCtxId;

        return (true, sampleRawCtx);
    }
}
