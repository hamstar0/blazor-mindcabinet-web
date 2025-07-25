﻿using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataObjects.Term;
using System;
using System.Data;


namespace MindCabinet.Data;


public partial class ServerDbAccess {
	public async Task<bool> InstallTermSets_Async( IDbConnection dbCon ) {
        await dbCon.ExecuteAsync( @"
            CREATE TABLE TermSet (
                SetId INT NOT NULL,
                TermId BIGINT NOT NULL,
                CONSTRAINT PK_Id PRIMARY KEY (SetId, TermId),
                CONSTRAINT FK_TermSetTermId FOREIGN KEY (TermId)
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

    //



    public async Task<long> CreateTermSet_Async(
                IDbConnection dbCon,
                params TermObject[] parameters ) {
        long newSetId = await dbCon.QuerySingleAsync<long>(
            @"INSERT INTO TermSetIdSupplier (Bogus) 
                    OUTPUT INSERTED.Id 
                    VALUES (null)"
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



    public async Task<IEnumerable<TermObject>> GetTermSet_Async( IDbConnection dbCon, int termSetId ) {
        IEnumerable<TermEntryData?> termSetRaw = await dbCon.QueryAsync<TermEntryData?>(
            @"SELECT Terms.Id, Terms.Term, Terms.ContextId, Terms.AliasId FROM Terms
                INNER JOIN TermSet ON (Terms.Id = TermSet.TermId)
                WHERE TermSet.SetId = @SetId",
            new { SetId = termSetId }
        );

        IList<TermObject> terms = new List<TermObject>( termSetRaw.Count() );

        foreach( TermEntryData? termRaw in termSetRaw ) {
            TermObject term = await termRaw!.Create_Async( dbCon, this );
            terms.Add( term );

            this.TermsById_Cache[ term.Id!.Value ] = term;
        }

        return terms;
    }
}
