using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_TermSets : IServerDataAccess {
    public const string TableName = "TermSet";
    // public const string IdSupplierTableName = "TermSetIdSupplier";


	public async Task<bool> Install_Async( IDbConnection dbCon ) {
        await dbCon.ExecuteAsync($@"
            CREATE TABLE {TableName} (
                SimplePostId BIGINT NOT NULL,
                TermId BIGINT NOT NULL,
                 CONSTRAINT PK_{TableName}_SetAndTermId PRIMARY KEY (SimplePostId, TermId),
                 CONSTRAINT FK_{TableName}_SimplePostId FOREIGN KEY (SimplePostId)
                    REFERENCES {ServerDataAccess_SimplePosts.TableName}(Id),
                 CONSTRAINT FK_{TableName}_TermId FOREIGN KEY (TermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id)
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

    //



    public async Task CreateForSimplePost_Async(
                IDbConnection dbCon,
                SimplePostId simplePostId,
                TermId[] termIds ) {
        // long newSetId = await dbCon.ExecuteScalarAsync<long>(
        //     $@"INSERT INTO {IdSupplierTableName} (Bogus) 
        //         VALUES (null);
        //     SELECT LAST_INSERT_ID();" //DEFAULT VALUES
        // );

        foreach(  long termId in termIds ) {
            await dbCon.ExecuteAsync(
                $@"INSERT INTO {TableName} (SimplePostId, TermId) 
                    VALUES (@SimplePostId, @TermId)",
                new {
                    SimplePostId = simplePostId,
                    TermId = termId,
                }
            );
        }
    }

    //



    public async Task<IEnumerable<TermObject.Raw>> GetTermSet_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                long simplePostId ) {
        IEnumerable<TermObject.Raw> termSetRaw = await dbCon.QueryAsync<TermObject.Raw>(
            $@"SELECT MyTerms.Id, MyTerms.Term, MyTerms.ContextId, MyTerms.AliasId
                FROM {ServerDataAccess_Terms.TableName} AS MyTerms
                INNER JOIN {TableName} AS MyTermSet ON (MyTerms.Id = MyTermSet.TermId)
                WHERE MyTermSet.SimplePostId = @SimplePostId",
            new { SimplePostId = simplePostId }
        );

        foreach( TermObject.Raw? termRaw in termSetRaw ) {
            termsData.TermsById_Cache[ termRaw.Id ] = termRaw;
        }

        return termSetRaw;
    }
}
