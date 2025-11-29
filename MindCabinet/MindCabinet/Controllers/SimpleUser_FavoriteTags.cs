using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet;


public partial class SimpleUserController : ControllerBase {
    [HttpPost(ClientDataAccess_SimplePosts_FavoriteTags.Get_Route)]
    public async Task<IEnumerable<long>> GetFavoriteTagIds_Async(
                ClientDataAccess_SimplePosts_FavoriteTags.Get_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        return await this.FavoriteTagsData.GetFavoriteTermIds_Async( dbCon, parameters );
    }


    [HttpPost(ClientDataAccess_SimplePosts_FavoriteTags.Add_Route)]
    public async Task AddFavoriteTagsById_Async(
                ClientDataAccess_SimplePosts_FavoriteTags.Add_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async();

        await this.FavoriteTagsData.AddSimpleUserFavoriteTermsById_Async( dbCon, parameters );
    }
}
