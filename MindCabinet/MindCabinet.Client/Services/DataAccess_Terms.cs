using System.Net.Http.Json;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet.Client.Data;



public partial class ClientDataAccess {
    public class GetTermsByCriteriaParams(
                string termPattern,
                TermEntry? context ) {
        public string TermPattern { get; } = termPattern;
        public TermEntry? Context { get; } = context;
    }

    //


    
    public async Task<IEnumerable<TermEntry>> GetTermsByCriteria_Async( GetTermsByCriteriaParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "Term/GetByCriteria", parameters );

        msg.EnsureSuccessStatusCode();

//string jsondata = await msg.Content.ReadAsStringAsync();
//Console.WriteLine( "Term/GetByCriteria: "+jsondata );
//return new List<TermEntry>();
        IEnumerable<TermEntry>? ret = await msg.Content.ReadFromJsonAsync<IEnumerable<TermEntry>>();
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
                TermEntry? context,
                TermEntry? alias ) {
        public string TermPattern { get; } = termPattern;
        public TermEntry? Context { get; } = context;
        public TermEntry? Alias { get; } = alias;
    }

    public class CreateTermReturn(
                bool isAdded,
                TermEntry term ) {
        public bool IsAdded { get; } = isAdded;
        public TermEntry Term { get; } = term;
    }
    
    public async Task<CreateTermReturn> CreateTerm_Async( CreateTermParams parameters ) {
        HttpResponseMessage msg = await this.Http.PostAsJsonAsync( "Term/Create", parameters );

        msg.EnsureSuccessStatusCode();

        CreateTermReturn? ret = await msg.Content.ReadFromJsonAsync<CreateTermReturn>();
        if( ret is null ) {
            throw new InvalidDataException( "Could not deserialize TermEntry" );
        }

        return ret;
    }
}
