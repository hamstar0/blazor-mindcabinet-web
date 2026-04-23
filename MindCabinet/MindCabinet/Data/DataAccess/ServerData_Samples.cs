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


public partial class ServerDataAccess_ServerData : IServerDataAccess {
    private async Task<bool> InstallSamples_Async(
                IDbConnection dbConnection,
                TermId usersConceptTermId ) {
        await this.Create_Async( dbConnection, usersConceptTermId );

        return true;
    }
}
