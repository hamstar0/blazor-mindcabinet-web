using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Terms_Sets {
	public async Task<bool> Install_Async( IDbConnection dbCon ) {
        await dbCon.ExecuteAsync(@"
            CREATE TABLE TermSet (
                SetId INT NOT NULL,
                TermId BIGINT NOT NULL,
                CONSTRAINT PK_Id PRIMARY KEY (SetId, TermId),
                CONSTRAINT FK_TermSetTermId FOREIGN KEY (TermId)
                    REFERENCES Terms(Id)
            )"
            //    ON DELETE CASCADE
            //    ON UPDATE CASCADE
        );
        await dbCon.ExecuteAsync( @"
            CREATE TABLE TermSetIdSupplier (
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
            @"INSERT INTO TermSetIdSupplier (Bogus) 
                VALUES (null);
            SELECT LAST_INSERT_ID();" //DEFAULT VALUES
        );

        foreach(  TermObject termEntry in parameters ) {
            await dbCon.ExecuteAsync(
                @"INSERT INTO TermSet (SetId, TermId) 
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



    public async Task<IEnumerable<TermObject>> GetTermSet_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                int termSetId ) {
        IEnumerable<ServerDataAccess_Terms.TermObjectDbData?> termSetRaw = await dbCon.QueryAsync<ServerDataAccess_Terms.TermObjectDbData?>(
            @"SELECT Terms.Id, Terms.Term, Terms.ContextId, Terms.AliasId FROM Terms
                INNER JOIN TermSet ON (Terms.Id = TermSet.TermId)
                WHERE TermSet.SetId = @SetId",
            new { SetId = termSetId }
        );

        var terms = new List<TermObject>( termSetRaw.Count() );

        foreach( ServerDataAccess_Terms.TermObjectDbData? termRaw in termSetRaw ) {
            TermObject term = await termRaw!.Create_Async( dbCon, termsData );
            terms.Add( term );

            termsData.TermsById_Cache[ term.Id ] = term;
        }

        return terms;
    }
}
