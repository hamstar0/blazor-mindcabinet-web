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


public partial class ServerDataAccess_Terms {
    private async Task<TermId> InstallSamples_Async(
                IDbConnection dbConnection ) {
        TermId userConceptTermId = (await this.Create_Async(
            dbCon: dbConnection,
            parameters: new ClientDataAccess_Terms.Create_Params {
                TermPattern = "Simple User",
                //Description = "A term that represents an instance of a 'SimpleUser'.",
                ContextId = null
            }
        )).TermRaw.Id;
        if( userConceptTermId == 0 ) {
            throw new Exception( "wtf" );
        }

        return userConceptTermId;
    }
}
