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


public partial class ClientSessionData(
            INetMode netMode,
            IServiceScopeFactory serviceScopeFactory ) {
    public class DataBundle( string sessionId, SimpleUserObject.ClientObject? userData, UserAppDataObject? userAppData ) {
        public string SessionId { get; set; } = sessionId;

        public SimpleUserObject.ClientObject? UserData { get; set; } = userData;

        public UserAppDataObject? UserAppData { get; set; } = userAppData;
    }



    private readonly INetMode NetMode = netMode;

    private readonly IServiceScopeFactory ServiceScopeFactory = serviceScopeFactory;

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
        using IServiceScope scope = this.ServiceScopeFactory.CreateScope();

        HttpClient? httpClient = scope.ServiceProvider.GetService<HttpClient>();
        if( httpClient is null ) {
            throw new InvalidOperationException( "HttpClient service not available in ClientSessionData." );
        }

        ClientDataAccess_Terms? termsData = scope.ServiceProvider.GetService<ClientDataAccess_Terms>();
        if( termsData is null ) {
            throw new InvalidOperationException( "ClientDataAccess_Terms service not available in ClientSessionData." );
        }

        ClientDataAccess_ClientSessionBundle? sessionBundle = scope.ServiceProvider.GetService<ClientDataAccess_ClientSessionBundle>();
        if( sessionBundle is null ) {
            throw new InvalidOperationException( "ClientDataAccess_ClientSessionBundle service not available in ClientSessionData." );
        }

        //

        await this.LoadData_Async( httpClient, termsData, sessionBundle, true );
    }
    
    private async Task LoadData_Async(
                HttpClient httpClient,
                ClientDataAccess_Terms termsData,
                ClientDataAccess_ClientSessionBundle sessionBundle,
                bool triggerEvents ) {
        ClientSessionData.DataBundle? userAndAppData = await sessionBundle.GetCurrent_Async( httpClient, termsData );
Console.WriteLine( "ClientSessionData.LoadData_Async: "+JsonSerializer.Serialize( userAndAppData ) );

        //

        this.Data = userAndAppData;

        if( triggerEvents ) {
            await this.TriggerUserAndAppDataLoaded_Async( userAndAppData );
            await this.TriggerPostsContextChanged_Async( this.Data.UserAppData?.PostsContext );
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
