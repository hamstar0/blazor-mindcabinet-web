using Dapper;
using MindCabinet.Client.Data;
using MindCabinet.Shared.DataEntries;
using System;
using System.Data;


namespace MindCabinet.Data;


public partial class ServerDataAccess {
	public async Task<bool> InstallTermSets_Async( IDbConnection dbCon ) {
        await dbCon.ExecuteAsync( @"
            CREATE TABLE TermSet (
                SetId INT NOT NULL,
                TermId BIGINT NOT NULL,
                CONSTRAINT PK_Id PRIMARY KEY (SetId, TermId),
                CONSTRAINT FK_TermId FOREIGN KEY (TermId)
                    REFERENCES Terms(Id)
            );"
            //    ON DELETE CASCADE
            //    ON UPDATE CASCADE
        );
        await dbCon.ExecuteAsync( @"
            CREATE TABLE TermSetIdSupplier (
                Id INT NOT NULL IDENTITY(1, 1) PRIMARY KEY CLUSTERED,
                Bogus BIT
            );"
        );
        
        return true;
    }



    public async Task<long> CreateTermSet_Async(
                IDbConnection dbCon,
                params TermEntry[] parameters ) {
        long newSetId = await dbCon.QuerySingleAsync<long>(
            @"INSERT INTO TermSetIdSupplier (Bogus)
                    OUTPUT INSERTED.Id
                    VALUES (null)"
        );

        foreach(  TermEntry termEntry in parameters ) {
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

    public async Task<IEnumerable<TermEntry>> GetTermSet_Async( IDbConnection dbCon, int termSetId ) {
        IEnumerable<TermEntryData?> termSetRaw = await dbCon.QueryAsync<TermEntryData?>(
            @"SELECT Terms.Id, Terms.Term, Terms.ContextId, Terms.AliasId FROM Terms
                INNER JOIN TermSet ON (Terms.Id = TermSet.TermId)
                WHERE TermSet.SetId = @SetId",
            new { SetId = termSetId }
        );

        IList<TermEntry> terms = new List<TermEntry>( termSetRaw.Count() );

        foreach( TermEntryData? termRaw in termSetRaw ) {
            TermEntry term = await termRaw!.CreateTerm_Async( dbCon, this );
            terms.Add( term );

            this.TermsById_Cache[ term.Id!.Value ] = term;
        }

        return terms;
    }
}
