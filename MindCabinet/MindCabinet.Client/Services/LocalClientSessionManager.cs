using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Bundled;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MindCabinet.Client.Services;


public partial class LocalClientSessionManager(
            INetMode netMode,
            IServiceScopeFactory serviceScopeFactory ) {
    public class DataBundle( string sessionId, SimpleUserObject.ClientObject? userData, UserAppDataObject? userAppData ) {
        public string SessionId { get; set; } = sessionId;

        public SimpleUserObject.ClientObject? UserData { get; set; } = userData;

        public UserAppDataObject? UserAppData { get; set; } = userAppData;
    }



    private readonly INetMode NetMode = netMode;

    private readonly IServiceScopeFactory ServiceScopeFactory = serviceScopeFactory;
    // private readonly IServiceProvider ServiceProvider = serviceProvider;

    public bool IsLoaded { get; private set; } = false;

    public bool IsLoading { get; private set; } = false;


    private DataBundle? Data;


    
    internal async Task<bool> Load_Async() {
        if( !this.NetMode.IsClientSide ) {
            //throw new InvalidOperationException( "Load_Async should only be called in client-side mode." );
            return false;
        }

        if( this.IsLoaded || this.IsLoading ) {
            return false;
        }
        this.IsLoading = true;

        //

        await this.LoadData_Async();

        //

        this.IsLoading = false;
        this.IsLoaded = true;

        return true;
    }
    
    private async Task LoadData_Async() {
        IServiceScope scope = this.ServiceScopeFactory.CreateScope();
        
        try {
            ClientDataAccess_Terms? termsDataSrc = scope.ServiceProvider.GetService<ClientDataAccess_Terms>();
            if( termsDataSrc is null ) {
                throw new InvalidOperationException( "ClientDataAccess_Terms service not available in ClientSessionData." );
            }

            ClientDataAccess_ClientSessionBundle? sessionBundleSrc = scope.ServiceProvider.GetService<ClientDataAccess_ClientSessionBundle>();
            if( sessionBundleSrc is null ) {
                throw new InvalidOperationException( "ClientDataAccess_ClientSessionBundle service not available in ClientSessionData." );
            }

            //

            await this.LoadData_Async( termsDataSrc, sessionBundleSrc, true );
        } finally {
            if( scope is IAsyncDisposable asyncDisposable ) {
                await asyncDisposable.DisposeAsync();
            } else {
                scope.Dispose();
            }
        }
    }
    
    private async Task LoadData_Async(
                ClientDataAccess_Terms termsDataSrc,
                ClientDataAccess_ClientSessionBundle sessionBundleDataSrc,
                bool triggerEvents ) {
        LocalClientSessionManager.DataBundle? userAndAppData = await sessionBundleDataSrc.GetCurrent_Async( termsDataSrc );
Console.WriteLine( "ClientSessionData.LoadData_Async: "+JsonSerializer.Serialize( userAndAppData ) );

        //

        this.Data = userAndAppData;

        if( triggerEvents ) {
            await this.TriggerUserAndAppDataLoaded_Async( userAndAppData );
            await this.TriggerPostsContextChanged_Async( this.Data.UserAppData?.CurrentPostsContext );
        }
    }
    

    private async Task UnloadData_Async( bool triggerEvents ) {
        this.Data = null;

        if( triggerEvents ) {
            await this.TriggerUserAndAppDataLoaded_Async( null );
            await this.TriggerPostsContextChanged_Async( null );
        }
    }
}
