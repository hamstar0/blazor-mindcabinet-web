using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using System.Data;
using System.Text;

namespace MindCabinet.Components;


public partial class App : ComponentBase {
    [Inject]
    private DbAccess Db { get; set; } = null!;

    [Inject]
    private ServerDataAccess_SimpleUsers_Sessions UserSessionsData { get; set; } = null!;

    [Inject]
    private ServerSessionData ServerSessionData { get; set; } = null!;



    protected async override Task OnInitializedAsync() {
        await base.OnInitializedAsync();

        using IDbConnection dbCon = await this.Db.GetDbConnection_Async();

        if( this.ServerSessionData.User is not null ) {
            await this.ServerSessionData.Visit_Async( dbCon, this.UserSessionsData );
        }
    }
}
