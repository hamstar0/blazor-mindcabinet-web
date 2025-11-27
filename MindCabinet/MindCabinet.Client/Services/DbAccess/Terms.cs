using System.Net.Http.Json;
using System.Text.Json;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Services.DbAccess;



public class ClientDbAccess_Terms {
    private HttpClient Http;



    internal ClientDbAccess_Terms( HttpClient http ) {
        this.Http = http;
    }


    public class GetByCriteria_Params(
                string termPattern,
                TermObject.Prototype? context ) {
        public string TermPattern { get; } = termPattern;
        public TermObject.Prototype? Context { get; } = context;
    }

    
    public const string GetByCriteria_Path = "Term";
    public const string GetByCriteria_Route = "GetByCriteria";
    
    public async Task<IEnumerable<TermObject>> GetByCriteria_Async( GetByCriteria_Params parameters ) {
//Console.WriteLine( "GetTermsByCriteria_Async "+JsonSerializer.Serialize(parameters) );
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{GetByCriteria_Path}/{GetByCriteria_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();

//string jsondata = await msg.Content.ReadAsStringAsync();
//Console.WriteLine( "Term/GetByCriteria: "+jsondata );
//return new List<TermEntry>();
        IEnumerable<TermObject>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<TermObject>>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize IEnumerable<TermEntry>" );
        }

//if( ret.Count() > 0 ) {
//Console.WriteLine( "GetTermsByCriteria_Async "+parameters.TermPattern+", "+parameters.Context?.ToString()
//    +", ["+string.Join( ",", ret.Select(t => t.Term) )+"] ("+ret.Count()+")" );
//}
        return ret;
    }


    public class Create_Params(
                string termPattern,
                TermObject? context,
                TermObject? alias ) {
        public string TermPattern { get; } = termPattern;
        public TermObject? Context { get; } = context;
        public TermObject? Alias { get; } = alias;
    }

    public class Create_Return(
                bool isAdded,
                TermObject term ) {
        public bool IsAdded { get; } = isAdded;
        public TermObject Term { get; } = term;
    }

    public const string Create_Path = "Term";
    public const string Create_Route = "Create";
    
    public async Task<Create_Return> Create_Async( Create_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            $"{Create_Path}/{Create_Route}",
            parameters
        );

        msg.EnsureSuccessStatusCode();

        Create_Return? ret = await msg.Content.ReadFromJsonAsync<Create_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize TermEntry" );
        }

        return ret;
    }
}
