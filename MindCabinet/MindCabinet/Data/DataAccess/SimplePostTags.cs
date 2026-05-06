using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimplePostTags : IServerDataAccess {
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
                $@"INSERT INTO {TableName} ({TableColumn_SimplePostId}, {TableColumn_TermId}) 
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
                SimplePostId id ) {
        if( id == 0 ) {
            throw new ArgumentException( "SimplePostId is not valid (must be non-zero)." );
        }

        IEnumerable<TermObject.Raw> termSetRaw = await dbCon.QueryAsync<TermObject.Raw>(
            $@"SELECT
                    MyTerms.{ServerDataAccess_Terms.TableColumn_Id},
                    MyTerms.{ServerDataAccess_Terms.TableColumn_Term},
                    MyTerms.{ServerDataAccess_Terms.TableColumn_ContextId},
                    MyTerms.{ServerDataAccess_Terms.TableColumn_AliasId}
                FROM {ServerDataAccess_Terms.TableName} AS MyTerms
                INNER JOIN {TableName} AS MyPostTags
                    ON (MyTerms.{ServerDataAccess_Terms.TableColumn_Id} = MyPostTags.{TableColumn_TermId})
                WHERE MyPostTags.{TableColumn_SimplePostId} = @SimplePostId",
            new { SimplePostId = (long)id }
        );

        return termSetRaw;
    }
}
