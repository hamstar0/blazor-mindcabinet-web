using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MindCabinet.Client.Components.Standard;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Components.Application.Pickers;



public partial class CurrentPostsQueryPicker : ComponentBase {
    public static CurrentPostsQueryPicker Main { get; private set; } = null!;



    public async Task SetSearch_Async( string search, bool forced ) {
        if( !forced && this.Disabled ) {
            return;
        }

        if( !forced && this.Value == search ) {
            return;
        }

        this.Value = search;
        this.SearchOptions = await this.GetQueriesFromSearch_Async( search );

        this.StateHasChanged();
    }


    public async Task<bool> PickQuery_Async( PostsQueryId id ) {
        PostsQueryObject? query = this.SearchOptions
            .FirstOrDefault( o => o.Id == id );
        if( query is null ) {
            return false;
        }

        this.SearchOptions = new List<PostsQueryObject>();
        this.Value = query.Name;

        await this.OnQueryPicked_Async( query );

        this.StateHasChanged();

        return true;
    }

    internal async Task ForceQueryPickedCallback_Async( PostsQueryObject query ) {
        await this.OnQueryPicked_Async( query );
    }
}
