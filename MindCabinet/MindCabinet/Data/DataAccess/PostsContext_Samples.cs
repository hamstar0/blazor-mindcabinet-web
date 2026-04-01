using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_PostsContexts : IServerDataAccess {
    private async Task<(bool success, PostsContextObject.Raw userPostsContext)> InstallSamples_Async(
                IDbConnection dbConnection,
                TermObject.Raw sampleTerm ) {
        var sampleRawEntry = PostsContextTermEntryObject.CreateRaw(
            userPostsContextId: (PostsContextId)(-1), // special case
            termId: sampleTerm.Id,
            priority: 1.0,
            isRequired: true
        );
        var protoSampleCtx = new PostsContextObject.Prototype {
            Id = (PostsContextId)(-1),  // special case
            Name = "Default Context",
            Description = "A sample user context.",
            Entries = [sampleRawEntry]
        };

        PostsContextId usrCtxId = (await this.Create_Async(
            dbCon: dbConnection,
            parameters: protoSampleCtx
        )).Id;

        protoSampleCtx.Id = usrCtxId;   // annoying!
        protoSampleCtx.Entries[0].UserPostsContextId = usrCtxId;   // annoying!

        var sampleRawCtx = protoSampleCtx.ToRaw( true );

        return (true, sampleRawCtx);
    }
}
