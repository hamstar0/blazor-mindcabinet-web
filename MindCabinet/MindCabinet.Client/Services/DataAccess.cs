using MindCabinet.Shared.DataEntries;


namespace MindCabinet.Client.Data;


public partial class ClientDataAccess {
    public HttpClient Http;


    public ClientDataAccess( HttpClient http ) {
        this.Http = http;
    }
}
