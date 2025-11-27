using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet;


public partial class SimpleUserController : ControllerBase {
    [HttpPost(ClientDbAccess.SimpleUser_GetFavoriteTags_Route)]
    public async Task<IEnumerable<long>> GetFavoriteTagIds_Async(
                ClientDbAccess.GetSimpleUserFavoriteTagsParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetFavoriteTermIds_Async( dbCon, parameters );
    }


    [HttpPost(ClientDbAccess.SimpleUser_AddFavoriteTags_Route)]
    public async Task AddFavoriteTags_Async( ClientDbAccess.AddSimpleUserFavoriteTagsParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        await this.DbAccess.AddSimpleUserFavoriteTerms_Async( dbCon, parameters );
    }
}
