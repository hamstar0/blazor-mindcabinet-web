using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_PostsContexts : IServerDataAccess {
    private async Task<(bool success, PostsContextObject.Raw postsContext)> InstallSamples_Async(
                IDbConnection dbConnection,
                TermObject.Raw sampleTerm ) {
        var sampleRawEntry = PostsContextTermEntryObject.CreateRaw(
            postsContextId: (PostsContextId)(-1), // special case
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

        PostsContextId ctxId = (await this.Create_Async(
            dbCon: dbConnection,
            parameters: protoSampleCtx
        )).Id;

        protoSampleCtx.Id = ctxId;   // annoying!
        protoSampleCtx.Entries[0].PostsContextId = ctxId;   // annoying!

        var sampleRawCtx = protoSampleCtx.ToRaw( true );

        return (true, sampleRawCtx);
    }
}
