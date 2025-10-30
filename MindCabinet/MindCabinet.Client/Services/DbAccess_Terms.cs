using System.Net.Http.Json;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Services;



public partial class ClientDbAccess {
    public class GetTermsByCriteriaParams(
                string termPattern,
                TermObject.Prototype? context ) {
        public string TermPattern { get; } = termPattern;
        public TermObject.Prototype? Context { get; } = context;
    }

    //


    
    public readonly static (string path, string route) Route_Term_GetByCriteria = ("Term", "GetByCriteria");
    
    public async Task<IEnumerable<TermObject>> GetTermsByCriteria_Async( GetTermsByCriteriaParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            ClientDbAccess.Route_Term_GetByCriteria.path + "/" + ClientDbAccess.Route_Term_GetByCriteria.route,
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


    public class CreateTermParams(
                string termPattern,
                TermObject? context,
                TermObject? alias ) {
        public string TermPattern { get; } = termPattern;
        public TermObject? Context { get; } = context;
        public TermObject? Alias { get; } = alias;
    }

    public class CreateTermReturn(
                bool isAdded,
                TermObject term ) {
        public bool IsAdded { get; } = isAdded;
        public TermObject Term { get; } = term;
    }

    public readonly static (string path, string route) Route_Term_Login = ("Term", "Create");
    
    public async Task<CreateTermReturn> CreateTerm_Async( CreateTermParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync(
            ClientDbAccess.Route_Term_Login.path + "/" + ClientDbAccess.Route_Term_Login.route,
            parameters
        );

        msg.EnsureSuccessStatusCode();

        CreateTermReturn? ret = await msg.Content.ReadFromJsonAsync<CreateTermReturn>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize TermEntry" );
        }

        return ret;
    }
}
