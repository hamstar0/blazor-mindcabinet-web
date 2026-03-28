using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserPostsContexts : IServerDataAccess {
    private async Task<(bool success, UserPostsContextObject.Raw userPostsContext)> InstallSamples_Async(
                IDbConnection dbConnection,
                TermObject.Raw sampleTerm ) {
        var sampleRawEntry = UserPostsContextTermEntryObject.CreateRaw(
            userPostsContextId: (UserPostsContextId)(-1), // special case
            termId: sampleTerm.Id,
            priority: 1.0,
            isRequired: true
        );
        var protoSampleCtx = new UserPostsContextObject.Prototype {
            Id = (UserPostsContextId)(-1),  // special case
            Name = "Default Context",
            Description = "A sample user context.",
            Entries = [sampleRawEntry]
        };

        UserPostsContextId usrCtxId = (await this.Create_Async(
            dbCon: dbConnection,
            parameters: protoSampleCtx
        )).Id;

        protoSampleCtx.Id = usrCtxId;   // annoying!
        protoSampleCtx.Entries[0].UserPostsContextId = usrCtxId;   // annoying!

        var sampleRawCtx = protoSampleCtx.ToRaw();

        return (true, sampleRawCtx);
    }
}
