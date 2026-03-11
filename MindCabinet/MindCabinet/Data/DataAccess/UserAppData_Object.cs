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


public partial class ServerDataAccess_UserAppData : IServerDataAccess {
    public async static Task<UserAppDataObject> ToObject_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_UserContexts userContextsData,
                UserAppDataObject.Raw dbEntry ) {
        Func<long[], Task<TermObject[]>> termsFactory = async termIds => {
            IEnumerable<TermObject.Raw> termRaws = await termsData.GetByIds_Async( dbCon, termIds );
            if( termRaws.Count() != termIds.Count() ) {
                throw new Exception( $"One or more terms not found." );
            }

            return await Task.WhenAll(
                termRaws.Select(
                    async termRaw => await ServerDataAccess_Terms.ToObject_Async( dbCon, termsData, termRaw )
                )
            );
        };

        Func<UserContextTermEntryObject.Raw[], Task<UserContextTermEntryObject[]>> ctxTermsFactory = async ctxTermEntries => {
            return await ServerDataAccess_UserContexts.ToTermEntriesDataObjects_Async( ctxTermEntries, termsFactory );
        };

        Func<long, Task<UserContextObject>> userContextFactory = async id => {
            UserContextObject.Raw? ctxRaw = await userContextsData.GetById_Async( dbCon, id );
            if( ctxRaw is null ) {
                throw new Exception( $"UserContext with id {id} not found." );
            }

            return await ctxRaw.CreateDataObject_Async( ctxTermsFactory );
        };

        return await dbEntry.CreateDataObject_Async( userContextFactory );
    }
}
