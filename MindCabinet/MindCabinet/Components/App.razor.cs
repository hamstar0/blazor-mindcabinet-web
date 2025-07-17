using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Data;
using System.Data;
using System.Text;

namespace MindCabinet.Components;


public partial class App : ComponentBase {
    [Inject]
    private ServerDbAccess Db { get; set; } = null!;

    [Inject]
    private ServerSessionData SessionData { get; set; } = null!;

    [Inject]
    public IServiceProvider ServiceProvider { get; set; } = null!;



    protected async override Task OnInitializedAsync() {
        await base.OnInitializedAsync();

        bool isClient = this.ServiceProvider.GetService<IsClient>() is not null;
        if( isClient ) {
            throw new Exception( "Not server!"  );
        }

        IDbConnection dbCon = await this.Db.ConnectDb_Async();

        await this.SessionData.Load_Async( dbCon );

        if( this.SessionData.SessionId is not null ) {
            if( isClient ) {
                throw new Exception( "test!" );
            } else {
                await this.SessionData.Visit_Async( dbCon );
            }
        }
    }
}
