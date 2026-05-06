using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimplePostTags : IServerDataAccess {
    public const string TableName = "SimplePostTags";
    public const string TableColumn_SimplePostId = "SimplePostId";
    public const string TableColumn_TermId = "TermId";


	public async Task<bool> Install_Async( IDbConnection dbCon ) {
        await dbCon.ExecuteAsync($@"
            CREATE TABLE {TableName} (
                {TableColumn_SimplePostId} BIGINT NOT NULL,
                {TableColumn_TermId} BIGINT NOT NULL,
                 CONSTRAINT PK_{TableName}_SetAndTermId PRIMARY KEY ({TableColumn_SimplePostId}, {TableColumn_TermId}),
                 CONSTRAINT FK_{TableName}_{TableColumn_SimplePostId} FOREIGN KEY ({TableColumn_SimplePostId})
                    REFERENCES {ServerDataAccess_SimplePosts.TableName}({ServerDataAccess_SimplePosts.TableColumn_Id}),
                 CONSTRAINT FK_{TableName}_{TableColumn_TermId} FOREIGN KEY ({TableColumn_TermId})
                    REFERENCES {ServerDataAccess_Terms.TableName}({ServerDataAccess_Terms.TableColumn_Id})
            )"
                // SetId INT NOT NULL,
            //    ON DELETE CASCADE
            //    ON UPDATE CASCADE
        );
        // await dbCon.ExecuteAsync( $@"
        //     CREATE TABLE {IdSupplierTableName} (
        //         Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
        //         Bogus BOOLEAN
        //     );"
        // );
        
        return true;
    }
}
