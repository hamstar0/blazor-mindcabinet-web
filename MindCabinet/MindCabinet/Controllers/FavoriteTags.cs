using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public partial class FavoriteTagsController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_FavoriteTags FavoriteTagsData;
    
    private readonly ServerSessionData ServerSessionData;



    public FavoriteTagsController(
                DbAccess dbAccess,
                ServerDataAccess_SimpleUsers simpleUsersData,
                ServerDataAccess_SimpleUsers_Sessions sessionsData,
                ServerDataAccess_FavoriteTags favoriteTagsData,
                ServerSessionData sessData ) {
        this.DbAccess = dbAccess;
        this.FavoriteTagsData = favoriteTagsData;
        this.ServerSessionData = sessData;
    }

    
    [HttpPost(ClientDataAccess_FavoriteTags.Get_Route)]
    public async Task<IEnumerable<long>> GetFavoriteTagIds_Async(
                ClientDataAccess_FavoriteTags.Get_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.FavoriteTagsData.GetFavoriteTermIds_Async( dbCon, parameters );
    }


    [HttpPost(ClientDataAccess_FavoriteTags.Add_Route)]
    public async Task AddFavoriteTagsById_Async(
                ClientDataAccess_FavoriteTags.Add_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        await this.FavoriteTagsData.AddSimpleUserFavoriteTermsById_Async( dbCon, parameters );
    }

    [HttpPost(ClientSessionData.Session_SetFavoriteTerm_Route)]
    public async Task<object> SetFavoriteTerm_Async( ClientSessionData.SetFavoriteTermSessionParams parameters ) {
        if( parameters.IsFavorite ) {
            IDbConnection dbConn = await this.DbAccess.GetDbConnection_Async();
            
            await this.ServerSessionData.AddFavoriteTerm_Async( dbConn, this.TermsData, parameters.TermId );
        } else {
            this.ServerSessionData.RemoveFavoriteTerm( parameters.TermId );
        }

        return new object();
    }
}
