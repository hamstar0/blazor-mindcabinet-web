using MindCabinet.Shared.DataObjects;


namespace MindCabinet.Client.Services;


public partial class ClientDbAccess {
    public HttpClient Http;


    public ClientDbAccess( HttpClient http ) {
        this.Http = http;
    }
}
