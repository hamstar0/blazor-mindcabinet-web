using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Terms : IServerDataAccess {
    public const string TableName = "Terms";
    public const string TableColumn_Id = "Id";
    public const string TableColumn_Term = "Term";
    public const string TableColumn_ContextId = "ContextId";
    public const string TableColumn_AliasId = "AliasId";

	public async Task<(bool success, TermId userConceptTermId)> Install_Async( IDbConnection dbCon ) {
        // todo: fulltext index on 'Term'
        await dbCon.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {TableColumn_Id} BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                {TableColumn_Term} VARCHAR(64) NOT NULL,
                {TableColumn_ContextId} BIGINT,
                {TableColumn_AliasId} BIGINT,
                 CONSTRAINT FK_{TableName}_{TableColumn_ContextId} FOREIGN KEY ({TableColumn_ContextId})
                    REFERENCES {TableName}({TableColumn_Id}),
                 CONSTRAINT FK_{TableName}_{TableColumn_AliasId} FOREIGN KEY ({TableColumn_AliasId})
                    REFERENCES {TableName}({TableColumn_Id})
            );"
        );

        TermId userConceptTermId = await this.InstallSamples_Async( dbCon );

        return (true, userConceptTermId);
    }


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
