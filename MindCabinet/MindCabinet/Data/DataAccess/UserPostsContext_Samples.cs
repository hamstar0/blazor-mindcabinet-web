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
                TermObject.Raw sampleTerm,
                SimpleUserId defaultUserId ) {
        var sampleRawEntry = new UserPostsContextTermEntryObject.Raw {
            TermId = sampleTerm.Id,
            Priority = 1.0,
            IsRequired = true
        };
        var sampleRawCtx = new UserPostsContextObject.Raw {
            Name = "Default Context",
            Description = "A sample user context.",
            Entries = [sampleRawEntry]
        };

        UserPostsContextId usrCtxId = (await this.Create_Async(
            dbCon: dbConnection,
            simpleUserId: defaultUserId,
            parameters: sampleRawCtx
        )).Id;

        sampleRawCtx.Id = usrCtxId;

        return (true, sampleRawCtx);
    }
}
