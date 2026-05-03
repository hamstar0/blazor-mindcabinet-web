using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimplePostTags : IServerDataAccess {
    public const string TableName = "SimplePostTags";


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
                SimplePostId id,
                TermId[] termIds ) {
        if( id == 0 ) {
            throw new ArgumentException( "SimplePostId is not valid (must be non-zero)." );
        }
        if( termIds.Any(t => t == 0) ) {
            throw new ArgumentException( "Some TermIds are not valid (must be non-zero)." );
        }

        // long newSetId = await dbCon.ExecuteScalarAsync<long>(
        //     $@"INSERT INTO {IdSupplierTableName} (Bogus) 
        //         VALUES (null);
        //     SELECT LAST_INSERT_ID();" //DEFAULT VALUES
        // );

        foreach( TermId termId in termIds ) {
            await dbCon.ExecuteAsync(
                $@"INSERT INTO {TableName} (SimplePostId, TermId) 
                    VALUES (@SimplePostId, @TermId)",
                new {
                    SimplePostId = (long)id,
                    TermId = (long)termId,
                }
            );
        }
    }

    //



    public async Task<IEnumerable<TermObject.Raw>> Get_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                SimplePostId id ) {
        if( id == 0 ) {
            throw new ArgumentException( "SimplePostId is not valid (must be non-zero)." );
        }

        IEnumerable<TermObject.Raw> termSetRaw = await dbCon.QueryAsync<TermObject.Raw>(
            $@"SELECT MyTerms.Id, MyTerms.Term, MyTerms.ContextId, MyTerms.AliasId
                FROM {ServerDataAccess_Terms.TableName} AS MyTerms
                INNER JOIN {TableName} AS MyTermSet ON (MyTerms.Id = MyTermSet.TermId)
                WHERE MyTermSet.SimplePostId = @SimplePostId",
            new { SimplePostId = (long)id }
        );

        return termSetRaw;
    }
}
