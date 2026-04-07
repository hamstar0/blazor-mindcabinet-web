using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Terms : IServerDataAccess {
	public static async Task<TermObject> ToObject_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                TermObject.Raw termRaw ) {
        Func<TermId, Task<TermObject.Raw>> termRawFactory = async ( TermId termId ) => {
            TermObject.Raw? term = await termsData.GetById_Async( dbCon, termId );
            if( term is null ) {
                throw new InvalidOperationException("Term not found");
            }
            return term;
        };

        return await termRaw.CreateDataObject_Async( termRawFactory );
    }
}
