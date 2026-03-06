using System.Net.Http.Json;
using System.Text.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Services.DbAccess;



public class ClientDataAccess_Terms( HttpClient http ) : IClientDataAccess {
    private HttpClient Http = http;



    public class GetByX_Return( IEnumerable<TermObject.DatabaseEntry> terms ) {
        public IEnumerable<TermObject.DatabaseEntry> Terms { get; } = terms;
    }

    public const string GetByIds_Path = "Term";
    public const string GetByIds_Route = "GetByIds";

    public async Task<GetByX_Return> GetByIds_Async( IEnumerable<long> termIds ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{GetByIds_Path}/{GetByIds_Route}",
            value: termIds
        );
        
        msg.EnsureSuccessStatusCode();

        GetByX_Return? ret = await msg.Content.ReadFromJsonAsync<GetByX_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize GetByX_Return" );
        }

        return ret;
    }


    public class GetByCriteria_Params(
                string termPattern,
                TermObject.Prototype? context ) {
        public string TermPattern { get; } = termPattern;
        public TermObject.Prototype? Context { get; } = context;
    }
    
    public const string GetByCriteria_Path = "Term";
    public const string GetByCriteria_Route = "GetByCriteria";
    
    public async Task<GetByX_Return> GetByCriteria_Async( GetByCriteria_Params parameters ) {
//Console.WriteLine( "GetTermsByCriteria_Async "+JsonSerializer.Serialize(parameters) );
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{GetByCriteria_Path}/{GetByCriteria_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

//string jsondata = await msg.Content.ReadAsStringAsync();
//Console.WriteLine( "Term/GetByCriteria: "+jsondata );
//return new List<TermEntry>();
        GetByX_Return? ret = await msg.Content.ReadFromJsonAsync<GetByX_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize GetByX_Return" );
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
                TermObject.DatabaseEntry term ) {
        public bool IsAdded { get; } = isAdded;
        public TermObject.DatabaseEntry Term { get; } = term;
    }

    public const string Create_Path = "Term";
    public const string Create_Route = "Create";
    
    public async Task<Create_Return> Create_Async( Create_Params parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            requestUri: $"{Create_Path}/{Create_Route}",
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        Create_Return? ret = await msg.Content.ReadFromJsonAsync<Create_Return>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize TermEntry" );
        }

        return ret;
    }
}
