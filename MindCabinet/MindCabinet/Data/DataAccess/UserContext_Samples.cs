using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserContext : IServerDataAccess {
    private async Task<bool> InstallSamples_Async(
                IDbConnection dbConnection,
                TermObject sampleTerm,
                long defaultUserId ) {
        var sampleRawEntry = new UserContextTermEntryObject.DatabaseEntry {
            TermId = sampleTerm.Id,
            Priority = 1.0,
            IsRequired = true
        };
        var sampleRawCtx = new UserContextObject.DatabaseEntry {
            Name = "Default Context",
            Description = "A sample user context.",
            Entries = new List<UserContextTermEntryObject.DatabaseEntry>() { sampleRawEntry }
        };

        long sampleCtxId = ( await this.Create_Async(
            dbCon: dbConnection,
            simpleUserId: defaultUserId,
            parameters: sampleRawCtx
        ) ).UserContextId;

        return true;
    }
}
