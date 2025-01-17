using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Data;
using MindCabinet.Data;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet;


[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase {
    private readonly ServerDataAccess Data;



    public PostController( ServerDataAccess data ) {
        this.Data = data;
    }


    [HttpPost("GetByCriteria")]
    public async Task<IEnumerable<PostEntry>> GetByCriteria_Async(
                ClientDataAccess.GetPostsByCriteriaParams parameters ) {
        return await this.Data.GetPostsByCriteria_Async( parameters );
    }

    [HttpPost("GetCountByCriteria")]
    public async Task<int> GetCountByCriteria_Async(
                ClientDataAccess.GetPostsByCriteriaParams parameters ) {
        return await this.Data.GetPostCountByCriteria_Async( parameters );
    }

    [HttpPost("Create")]
    public async Task<PostEntry> Create_Async( ClientDataAccess.CreatePostParams parameters ) {
        return await this.Data.CreatePost_Async( parameters );
    }
}
