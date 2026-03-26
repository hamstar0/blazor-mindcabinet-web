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


public partial class ServerDataAccess_UserPostsContexts : IServerDataAccess {
    public static async Task<UserPostsContextTermEntryObject[]> ToTermEntriesDataObjects_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                UserPostsContextTermEntryObject.Raw[] entriesRaw ) {
        IEnumerable<TermObject.Raw> termRaws = await termsData.GetByIds_Async( dbCon, entriesRaw.Select(e => e.TermId) );

        return await Task.WhenAll( entriesRaw.Select( async entryRaw => {
            TermObject term = await ServerDataAccess_Terms.ToObject_Async(dbCon, termsData, termRaws.First( t => t.Id == entryRaw.TermId) );
            return new UserPostsContextTermEntryObject( term, entryRaw.Priority, entryRaw.IsRequired );
        } ) );
    }
}
