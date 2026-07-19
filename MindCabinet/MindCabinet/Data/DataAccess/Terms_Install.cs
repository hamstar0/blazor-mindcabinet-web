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
    public const string TableColumn_Creator = "Creator";
    public const string TableColumn_Term = "Term";
    public const string TableColumn_Abbreviation = "Abbreviation";
    public const string TableColumn_Description = "Description";
    public const string TableColumn_ContextId = "ContextId";
    public const string TableColumn_AliasId = "AliasId";

    public readonly (string column, string def)[] TableColumns = [
        ( TableColumn_Id, "BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY" ),
        ( TableColumn_Creator, "BIGINT NOT NULL" ),
        ( TableColumn_Term, "VARCHAR(64) NOT NULL" ),
        ( TableColumn_Abbreviation, "VARCHAR(64)" ),
        ( TableColumn_Description, "MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci" ),
        ( TableColumn_ContextId, "BIGINT" ),
        ( TableColumn_AliasId, "BIGINT" )
    ];

	public async Task<bool> Install_Async( IDbConnection dbCon ) {
        // todo: fulltext index on 'Term'
        await dbCon.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {string.Join(",\n                ", TableColumns.Select(kv => kv.column+" "+kv.def))}
                 CONSTRAINT FK_{TableName}_{TableColumn_Creator} FOREIGN KEY ({TableColumn_Creator})
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}({ServerDataAccess_SimpleUsers.TableColumn_Id}),
                 CONSTRAINT FK_{TableName}_{TableColumn_ContextId} FOREIGN KEY ({TableColumn_ContextId})
                    REFERENCES {TableName}({TableColumn_Id}),
                 CONSTRAINT FK_{TableName}_{TableColumn_AliasId} FOREIGN KEY ({TableColumn_AliasId})
                    REFERENCES {TableName}({TableColumn_Id}),
                 INDEX IDX_{TableColumn_Term} ({TableColumn_Term})
            );"
                // {TableColumn_Id} BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                // {TableColumn_Term} VARCHAR(64) NOT NULL,
                // {TableColumn_Abbreviation} VARCHAR(64),
                // {TableColumn_Description} MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
                // {TableColumn_ContextId} BIGINT,
                // {TableColumn_AliasId} BIGINT,
        );

        return true;
    }

	public async Task<(bool success, TermId userConceptTermId)> Install_After_Async(
                IDbConnection dbCon,
                SimpleUserId creator ) {
        TermId userConceptTermId = await this.InstallSamples_Async( dbCon, creator );

        return (true, userConceptTermId);
    }


    private async Task<TermId> InstallSamples_Async(
                IDbConnection dbConnection,
                SimpleUserId creator ) {
        TermId userConceptTermId = (await this.Create_Async(
            dbCon: dbConnection,
            creator: creator,
            parameters: new ClientDataAccess_Terms.IAPI.Create_Params {
                TermBody = "Simple User",
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
