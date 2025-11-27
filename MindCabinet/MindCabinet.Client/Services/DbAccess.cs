using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services;


public partial class ClientDbAccess {
    private HttpClient Http;


    public ClientDbAccess( HttpClient http ) {
        this.Http = http;
        this.Terms = new ClientDbAccess_Terms( this.Http );
        this.SimpleUsers = new ClientDbAccess_SimpleUsers( this.Http );
    }
}
