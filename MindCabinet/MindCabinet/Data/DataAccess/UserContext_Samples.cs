using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserContexts : IServerDataAccess {
    private async Task<(bool success, UserContextObject userContext)> InstallSamples_Async(
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

        long usrCtxId = (await this.Create_Async(
            dbCon: dbConnection,
            simpleUserId: defaultUserId,
            parameters: sampleRawCtx
        )).UserContextId;

        UserContextObject usrCtx = new UserContextObject(
            id: usrCtxId,
            name: sampleRawCtx.Name,
            description: sampleRawCtx.Description,
            entries: new List<UserContextTermEntryObject>() {
                new UserContextTermEntryObject(
                    term: sampleTerm,
                    priority: sampleRawEntry.Priority,
                    isRequired: sampleRawEntry.IsRequired
                )
            }
        );

        return (true, usrCtx);
    }
}
