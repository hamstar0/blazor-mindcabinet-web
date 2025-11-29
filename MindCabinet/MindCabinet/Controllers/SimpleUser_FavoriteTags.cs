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
    [HttpPost(ClientDbAccess_SimplePosts_FavoriteTags.Get_Route)]
    public async Task<IEnumerable<long>> GetFavoriteTagIds_Async(
                ClientDbAccess_SimplePosts_FavoriteTags.Get_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetFavoriteTermIds_Async( dbCon, parameters );
    }


    [HttpPost(ClientDbAccess_SimplePosts_FavoriteTags.Add_Route)]
    public async Task AddFavoriteTagsById_Async(
                ClientDbAccess_SimplePosts_FavoriteTags.Add_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        await this.DbAccess.AddSimpleUserFavoriteTermsById_Async( dbCon, parameters );
    }
}
