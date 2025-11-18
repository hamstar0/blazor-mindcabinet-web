using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    public ReadOnlyCollection<TermObject> FavoriteTerms {
        get => this.Data?.FavoriteTerms.AsReadOnly() ?? new ReadOnlyCollection<TermObject>([]);

    }
    public ReadOnlyCollection<TermObject> RecentTerms {
        get => this.Data?.RecentTerms.AsReadOnly() ?? new ReadOnlyCollection<TermObject>([]);
    }



    public class SetFavoriteTermSessionParams(
                long termId,
                bool isFavorite) {
        public long TermId { get; } = termId;
        public bool IsFavorite { get; } = isFavorite;
    }

    public const string Session_SetFavoriteTerm_Path = "Session";
    public const string Session_SetFavoriteTerm_Route = "SetFavoriteTerm";

    public async Task SetFavoriteTerm_Async( TermObject term, bool isFavorite ) {
        if( this.Data is null ) {
            throw new InvalidOperationException( "ClientSessionData is not loaded." );
        }

        if( isFavorite ) {
            if( !this.Data.FavoriteTerms.Any(t => t.Id == term.Id) ) {
                this.Data.FavoriteTerms.Add( term );
            }
        } else {
            this.Data.FavoriteTerms.Remove( term );
        }
        
        var response = await this.Http.PostAsJsonAsync(
            $"{Session_SetFavoriteTerm_Path}/{Session_SetFavoriteTerm_Route}",
            new SetFavoriteTermSessionParams( term.Id, isFavorite )
        );
    }
}
