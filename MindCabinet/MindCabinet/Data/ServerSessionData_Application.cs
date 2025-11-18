using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Security.Cryptography;

namespace MindCabinet.Data;



public partial class ServerSessionData {
    public ReadOnlyCollection<TermObject> FavoriteTerms => this._FavoriteTerms.AsReadOnly();
    private List<TermObject> _FavoriteTerms = new List<TermObject>();
    
    public ReadOnlyCollection<TermObject> TermHistory => this._TermHistory.AsReadOnly();
    private List<TermObject> _TermHistory = new List<TermObject>();



    public async Task AddFavoriteTerm( IDbConnection dbCon, long termId ) {
        if( this._FavoriteTerms.Any( t => t.Id == termId ) ) {
            return;
        }

        TermObject? term = await this.Db.GetTerm_Async( dbCon, termId );
        if( term is null ) {
            throw new Exception( $"Term with ID {termId} not found." );
        }

        this._FavoriteTerms.Add( term );
    }

    public void RemoveFavoriteTerm( long termId ) {
        TermObject? term = this._FavoriteTerms.FirstOrDefault( t => t.Id == termId );
        if( term is not null ) {
            this._FavoriteTerms.Remove( term );
        }
    }
    

    public void AddTermsToHistory( IEnumerable<TermObject> terms ) {
        foreach( TermObject term in terms ) {
            if( this._TermHistory.Count >= 100 ) {
                this._TermHistory.RemoveAt( 0 );
            }

            this._TermHistory.Add( term );
        }
    }

    public List<TermObject> GetTopTerms( int within ) {
        return this._TermHistory
            .GroupBy( t => t.Id )
            .OrderByDescending( g => g.Count() )
            .Take( within )
            .Select( g => g.First() )
            .ToList();
    }
}
