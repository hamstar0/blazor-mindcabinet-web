using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;
using System;
using System.Data;
using System.Text;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Terms : IServerDataAccess {
    public async Task<ClientDataAccess_Terms.IAPI.CreateForCurrentUser_Return> Create_Async(
                IDbConnection dbCon,
                SimpleUserId creator,
                ClientDataAccess_Terms.IAPI.CreateForCurrentUser_Params parameters ) {
        if( !TermObject.ValidateTerm(parameters.TermBody) ) {
            throw new ArgumentException( "Term is not valid." );
        }

		IEnumerable<TermObject.Raw> matchingTerms = await this.GetTermsByCriteria_Async(
            dbCon: dbCon,
			parameters: new ClientDataAccess_Terms.IAPI.GetByCriteria_Params {
				TermPattern = parameters.TermBody,
                ContextTermId = parameters.ContextId,
                ContextTermPattern = null
            }
		);
		if( matchingTerms.Count() == 1 ) {
			return new ClientDataAccess_Terms.IAPI.CreateForCurrentUser_Return { IsAdded = false, TermRaw = matchingTerms.First() };
		} else if( matchingTerms.Count() >= 2 ) {
            throw new Exception( "Multiple matching terms found." );
        }

        long newId = await dbCon.ExecuteScalarAsync<long>(
            $@"INSERT INTO {TableName}
                (
                    {TableColumn_Term},
                    {TableColumn_Creator},
                    {TableColumn_Abbreviation},
                    {TableColumn_Description},
                    {TableColumn_ContextId},
                    {TableColumn_AliasId}
                ) 
                VALUES (@Term, @Creator, @Abbreviation, @Description, @ContextId, @AliasId);
            SELECT LAST_INSERT_ID();",
            new {
                Term = parameters.TermBody,
                Creator = creator,
                Abbreviation = parameters.Abbreviation,
                Description = parameters.Description,
                ContextId = parameters.ContextId,
                AliasId = parameters.AliasId,
            }
        );
        if( newId == 0 ) {
            throw new Exception( "Could not declare new term." );
        }

        var newTermRaw = TermObject.CreateRaw(
			id: (TermId)newId,
            creator: creator,
			term: parameters.TermBody,
            abbreviation: parameters.Abbreviation,
            description: parameters.Description,
			contextId: parameters.ContextId,
			aliasId: parameters.AliasId
		);

        //

        ServerDataAccess_Terms.Cache_ById.Set(
            key: (TermId)newId,
            value: newTermRaw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return new ClientDataAccess_Terms.IAPI.CreateForCurrentUser_Return { IsAdded = true, TermRaw = newTermRaw };
    }


    public async Task<bool> Remove_Async( TermId id ) {
        throw new NotImplementedException();

        // -- Start transaction for safe removal
        // START TRANSACTION;

        // -- 1. Remove user term favorites
        // DELETE FROM UserTermFavorites 
        // WHERE FavTermId = @TermId;

        // -- 2. Remove user terms history
        // DELETE FROM UserTermsHistory 
        // WHERE TermId = @TermId;

        // -- 3. Remove simple post tags
        // DELETE FROM SimplePostTags 
        // WHERE TermId = @TermId;

        // -- 4. Remove posts context term entries (todo: also delete PostsContextEntry with no terms)
        // DELETE FROM PostsContextTermEntry 
        // WHERE TermId = @TermId;

        // -- 5. Handle UserAppData defaults - either update or delete
        // -- Option A: Set to NULL or a fallback term
        // UPDATE UserAppData 
        // SET UserDefaultTermId = NULL 
        // WHERE UserDefaultTermId = @TermId;

        // -- 6. Handle terms that reference this term as Context
        // DELETE FROM Terms 
        // WHERE ContextId = @TermId;

        // -- 7. Handle terms that reference this term as Alias
        // DELETE FROM Terms 
        // WHERE AliasId = @TermId;

        // -- 8. Finally, delete the term itself
        // DELETE FROM Terms 
        // WHERE Id = @TermId;

        // COMMIT;
    }


    public async Task<bool> Update_Async(
                IDbConnection dbCon,
                SimpleUserId creator,
                ClientDataAccess_Terms.IAPI.UpdateForCurrentUser_Params parameters ) {
        if( parameters.Id == 0 ) {
            throw new ArgumentException( "Term is not valid." );
        }
        if( parameters.TermBody is not null && !TermObject.ValidateTerm(parameters.TermBody) ) {
            throw new ArgumentException( "Term is not valid." );
        }
        if( parameters.Abbreviation is not null && !TermObject.ValidateTerm(parameters.Abbreviation) ) {
            throw new ArgumentException( "Abbreviation is not valid." );
        }
        if( parameters.ContextId is not null && parameters.ContextId <= 0 ) {
            throw new ArgumentException( "Context is not valid." );
        }
        if( parameters.AliasId is not null && parameters.AliasId <= 0 ) {
            throw new ArgumentException( "Alias is not valid." );
        }

        StringBuilder sql = new StringBuilder( $"UPDATE {TableName} SET" );
        var sqlParams = new DynamicParameters();
        bool needsComma = false;

        if( parameters.TermBody is not null ) {
            sql.Append( $" {TableColumn_Term} = @Term" );
            sqlParams.Add( "@Term", parameters.TermBody );
            needsComma = true;
        }
        if( parameters.Abbreviation is not null ) {
            sql.Append( $"{(needsComma ? ", " : " ")}{TableColumn_Abbreviation} = @Abbreviation" );
            sqlParams.Add( "@Abbreviation", parameters.Abbreviation );
            needsComma = true;
        }
        if( parameters.ContextId is not null ) {
            sql.Append( $"{(needsComma ? ", " : " ")}{TableColumn_ContextId} = @ContextId" );
            sqlParams.Add( "@ContextId", parameters.ContextId );
            needsComma = true;
        }
        if( parameters.AliasId is not null ) {
            sql.Append( $"{(needsComma ? ", " : " ")}{TableColumn_AliasId} = @AliasId" );
            sqlParams.Add( "@AliasId", parameters.AliasId );
            needsComma = true;
        }

        sql.Append( $" WHERE {TableColumn_Id} = @Id;" );

        try {
            await dbCon.ExecuteAsync( sql.ToString(), sqlParams );
        } catch( Exception e ) { //when ( ex.Number == 1062 ) {
            throw new InvalidOperationException( $"Record could not be updated ({parameters.Id})", e );
        }

        return true;
    }
}
