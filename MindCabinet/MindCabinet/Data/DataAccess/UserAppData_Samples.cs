using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    private async Task<bool> InstallSamples_Async(
                IDbConnection dbConnection,
                SimpleUserId defaultUserId,
                PostsContextId sampleContextId ) {
        var sampleRaw = UserAppDataObject.CreateRaw(
            simpleUserId: defaultUserId,
            postsContextId: sampleContextId
        );

        await this.Create_Async(
            dbCon: dbConnection,
            simpleUserId: defaultUserId,
            postsContextId: sampleContextId
        );

        return true;
    }
}
