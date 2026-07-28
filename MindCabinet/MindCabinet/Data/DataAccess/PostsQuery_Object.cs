using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Shared.Utility;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_PostsQuery : IServerDataAccess {
    public static async Task<PostsQueryObject> ToDataObject_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsDataSrc,
                PostsQueryObject.Raw raw ) {
        return await raw.ToDataObject_Async( queryTermsFactory: async queryTermEntries => {
            return await ServerDataAccess_PostsQuery.ToTermEntriesDataObjects_Async(
                dbCon: dbCon,
                termsDataSrc: termsDataSrc,
                entriesRaw: raw.Entries
            );
        } );
    }


    public static async Task<PostsQueryTermEntryObject[]> ToTermEntriesDataObjects_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsDataSrc,
                PostsQueryTermEntryObject.Raw[] entriesRaw ) {
        IEnumerable<TermObject.Raw> termRaws = await termsDataSrc
            .GetByIds_Async( dbCon, entriesRaw.Select(e => e.TermId) );

        Func<PostsQueryTermEntryObject.Raw, Task<PostsQueryTermEntryObject>> getTermEntry = async entryRaw => {
            TermObject term = await ServerDataAccess_Terms.ToDataObject_Async(
                dbCon: dbCon,
                termsDataSrc: termsDataSrc,
                termRaw: termRaws.First( t => t.Id == entryRaw.TermId )
            );

            return new PostsQueryTermEntryObject( term, entryRaw.Priority, entryRaw.IsRequired );
        };

        var entries = new PostsQueryTermEntryObject[ entriesRaw.Length ];
        int i = 0;
        foreach( PostsQueryTermEntryObject.Raw entryRaw in entriesRaw ) {
            entries[i++] = await getTermEntry( entryRaw );
        }
        return entries;
        // Can't just use Task.WhenAll?
    }
}
