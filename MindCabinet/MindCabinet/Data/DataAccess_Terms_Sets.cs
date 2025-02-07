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
                Id INT NOT NULL PRIMARY KEY CLUSTERED
            );"
        );

        return true;
    }



    public async Task<long> CreateTermSet_Async(
                IDbConnection dbCon,
                params TermEntry[] parameters ) {
        long newSetId = await dbCon.QuerySingleAsync(
            @"INSERT INTO TermSetIdSupplier DEFAULT VALUES
                OUTPUT INSERTED.Id"
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

    public async Task<IEnumerable<TermEntry>> GetTermSet_Async( IDbConnection dbCon, int id ) {
        IEnumerable<TermEntryData?> termSetRaw = await dbCon.QueryAsync<TermEntryData?>(
            @"SELECT A.Id A.Term A.ContextId A.AliasId FROM Terms AS A
                INNER JOIN TermSet AS B ON (A.Id = B.TermId)
                WHERE B.Id = @Id",
            new { Id = id }
        );

        IList<TermEntry> terms = new List<TermEntry>( termSetRaw.Count() );

        foreach( TermEntryData? termRaw in termSetRaw ) {
            TermEntry term = await termRaw!.CreateTerm_Async( dbCon, this );
            terms.Add( term );

            this.TermsById_Cache.Add( term.Id!.Value, term );
        }

        return terms;
    }
}
