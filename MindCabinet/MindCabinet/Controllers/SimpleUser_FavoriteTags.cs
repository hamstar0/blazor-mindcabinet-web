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
    [HttpPost(ClientDbAccess.SimpleUser_GetFavoriteTagIds_Route)]
    public async Task<IEnumerable<long>> GetFavoriteTagIds_Async(
                ClientDbAccess.GetSimpleUserFavoriteTagIdsParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        return await this.DbAccess.GetFavoriteTermIds_Async( dbCon, parameters );
    }


    [HttpPost(ClientDbAccess.SimpleUser_AddFavoriteTagsById_Route)]
    public async Task AddFavoriteTagsById_Async(
                ClientDbAccess.AddSimpleUserFavoriteTagsByIdParams parameters ) {
        using IDbConnection dbCon = await this.DbAccess.ConnectDb_Async();

        await this.DbAccess.AddSimpleUserFavoriteTermsById_Async( dbCon, parameters );
    }
}
