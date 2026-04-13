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


public partial class ServerDataAccess_PostsContexts : IServerDataAccess {
    public static async Task<PostsContextTermEntryObject[]> ToTermEntriesDataObjects_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                PostsContextTermEntryObject.Raw[] entriesRaw ) {
        IEnumerable<TermObject.Raw> termRaws = await termsData.GetByIds_Async( dbCon, entriesRaw.Select(e => e.TermId) );

        Func<PostsContextTermEntryObject.Raw, Task<PostsContextTermEntryObject>> getTermEntry = async entryRaw => {
            TermObject term = await ServerDataAccess_Terms.ToDataObject_Async(
                dbCon: dbCon,
                termsData: termsData,
                termRaw: termRaws.First( t => t.Id == entryRaw.TermId )
            );

            return new PostsContextTermEntryObject( term, entryRaw.Priority, entryRaw.IsRequired );
        };

        var entries = new PostsContextTermEntryObject[ entriesRaw.Length ];
        int i = 0;
        foreach( PostsContextTermEntryObject.Raw entryRaw in entriesRaw ) {
            entries[i++] = await getTermEntry( entryRaw );
        }
        return entries;
        // Can't just use Task.WhenAll?
    }
}
