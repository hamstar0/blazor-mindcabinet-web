using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_UserContexts : IServerDataAccess {
    public static async Task<UserContextTermEntryObject[]> ToTermEntriesDataObjects_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                UserContextTermEntryObject.Raw[] entriesRaw ) {
    }
}
