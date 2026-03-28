using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserPostsContext;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    private async Task<bool> InstallSamples_Async(
                IDbConnection dbConnection,
                SimpleUserId defaultUserId,
                UserPostsContextId sampleContextId ) {
        var sampleRaw = UserAppDataObject.CreateRaw(
            simpleUserId: defaultUserId,
            userPostsContextId: sampleContextId
        );

        await this.Create_Async(
            dbCon: dbConnection,
            simpleUserId: defaultUserId,
            userPostsContextId: sampleContextId
        );

        return true;
    }
}
