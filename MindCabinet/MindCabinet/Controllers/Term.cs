using Microsoft.AspNetCore.Mvc;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects.Term;
using System.Data;


namespace MindCabinet.Controllers;


[ApiController]
[Route("[controller]")]
public class TermController : ControllerBase {
    private readonly DbAccess DbAccess;

    private readonly ServerDataAccess_Terms TermsData;



    public TermController( DbAccess dbAccess, ServerDataAccess_Terms termsData ) {
        this.DbAccess = dbAccess;
        this.TermsData = termsData;
    }


    [HttpPost(ClientDataAccess_Terms.GetByCriteria_Route)]
    public async Task<ClientDataAccess_Terms.GetByX_Return> GetByCriteria_Async(
                ClientDataAccess_Terms.GetByCriteria_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<TermObject.Raw> terms =  await this.TermsData.GetTermsByCriteria_Async( dbCon, parameters );

        return new ClientDataAccess_Terms.GetByX_Return( terms );
    }

    [HttpPost(ClientDataAccess_Terms.GetByIds_Route)]
    public async Task<ClientDataAccess_Terms.GetByX_Return> GetByIds_Async(
                IEnumerable<TermId> ids ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        IEnumerable<TermObject.Raw> terms = await this.TermsData.GetByIds_Async( dbCon, ids );

        return new ClientDataAccess_Terms.GetByX_Return( terms );
    }

    [HttpPost(ClientDataAccess_Terms.Create_Route)]
    public async Task<ClientDataAccess_Terms.Create_Return> Create_Async(
                ClientDataAccess_Terms.Create_Params parameters ) {
        using IDbConnection dbCon = await this.DbAccess.GetDbConnection_Async( true );

        return await this.TermsData.Create_Async( dbCon, parameters );
    }
}
