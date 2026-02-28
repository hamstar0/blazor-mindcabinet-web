using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_TermSets : IServerDataAccess {
    public const string TableName = "TermSet";
    public const string IdSupplierTableName = "TermSetIdSupplier";


	public async Task<bool> Install_Async( IDbConnection dbCon ) {
        await dbCon.ExecuteAsync($@"
            CREATE TABLE {TableName} (
                SetId INT NOT NULL,
                TermId BIGINT NOT NULL,
                CONSTRAINT PK_{TableName}_SetAndTermId PRIMARY KEY (SetId, TermId),
                CONSTRAINT FK_{TableName}_TermId FOREIGN KEY (TermId)
                    REFERENCES {ServerDataAccess_Terms.TableName}(Id)
            )"
            //    ON DELETE CASCADE
            //    ON UPDATE CASCADE
        );
        await dbCon.ExecuteAsync( $@"
            CREATE TABLE {IdSupplierTableName} (
                Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                Bogus BOOLEAN
            );"
        );
        
        return true;
    }

    //



    public async Task<long> Create_Async(
                IDbConnection dbCon,
                params TermObject[] parameters ) {
        long newSetId = await dbCon.ExecuteScalarAsync<long>(
            $@"INSERT INTO {IdSupplierTableName} (Bogus) 
                VALUES (null);
            SELECT LAST_INSERT_ID();" //DEFAULT VALUES
        );

        foreach(  TermObject termEntry in parameters ) {
            await dbCon.ExecuteAsync(
                $@"INSERT INTO {TableName} (SetId, TermId) 
                    VALUES (@SetId, @TermId)",
                new {
                    SetId = newSetId,
                    TermId = termEntry.Id,
                }
            );
        }

        return newSetId;
    }

    //



    public async Task<TermSetObject.DatabaseEntry> GetTermSet_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                long termSetId ) {
        IEnumerable<TermObject.DatabaseEntry?> termSetRaw = await dbCon.QueryAsync<TermObject.DatabaseEntry?>(
            $@"SELECT MyTerms.Id, MyTerms.Term, MyTerms.ContextId, MyTerms.AliasId
                FROM {ServerDataAccess_Terms.TableName} AS MyTerms
                INNER JOIN {TableName} AS MyTermSet ON (MyTerms.Id = MyTermSet.TermId)
                WHERE MyTermSet.SetId = @SetId",
            new { SetId = termSetId }
        );

        var terms = new List<TermObject>( termSetRaw.Count() );

        foreach( TermObject.DatabaseEntry? termRaw in termSetRaw ) {
            TermObject term = await ServerDataAccess_Terms.CreateTermObject_Async(dbCon, termsData, termRaw! );
            terms.Add( term );

            termsData.TermsById_Cache[ term.Id ] = term;
        }

        return terms;
    }
}
