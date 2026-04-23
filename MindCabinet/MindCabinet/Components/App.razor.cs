using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using MindCabinet.Data.DataAccess;
using MindCabinet.Services;
using System.Data;
using System.Text;

namespace MindCabinet.Components;


public partial class App : ComponentBase {
    [Inject]
    private DbAccess Db { get; set; } = null!;

    [Inject]
    private ServerDataAccess_SimpleUserSessions UserSessionsData { get; set; } = null!;

    [Inject]
    private ServerSessionManager SessionManager { get; set; } = null!;



    protected async override Task OnInitializedAsync() {
        await base.OnInitializedAsync();

        using IDbConnection dbCon = await this.Db.GetDbConnection_Async( true );

        if( this.SessionManager.UserOfSession is not null ) {
            await this.SessionManager.Visit_Async( dbCon, this.UserSessionsData );
        }
    }
}
