using MindCabinet.Shared.DataObjects;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MindCabinet.Client.Services;


public partial class ClientSessionData {
    public ReadOnlyCollection<long> FavoriteTermIds { get => this.Data?.FavoriteTermIds.AsReadOnly() ?? new ReadOnlyCollection<long>([]); }



    public const string Session_SetFavoriteTerm_Path = "Session";
    public const string Session_SetFavoriteTerm_Route = "SetFavoriteTerm";

    public async Task SetFavoriteTerm_Async( long termId, bool isFavorite ) {
        if( this.Data is null ) {
            throw new InvalidOperationException( "ClientSessionData is not loaded." );
        }

        if( isFavorite ) {
            if( !this.Data.FavoriteTermIds.Contains( termId ) ) {
                this.Data.FavoriteTermIds.Add( termId );
            }
        } else {
            this.Data.FavoriteTermIds.Remove( termId );
        }

        f
    }
}
