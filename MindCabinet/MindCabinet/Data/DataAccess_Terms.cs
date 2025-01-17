using MindCabinet.Client.Data;
using MindCabinet.Shared.DataEntries;


namespace MindCabinet.Data;


public partial class ServerDataAccess {
    private long CurrentTermId = 0;
    private IDictionary<long, TermEntry> Terms = new Dictionary<long, TermEntry>();



    public async Task<IEnumerable<TermEntry>> GetTermsByCriteria_Async(
                ClientDataAccess.GetTermsByCriteriaParams parameters ) {
		var terms = this.Terms.Values
			.Where( t => t.DeepTest(parameters.TermPattern, parameters.Context) );

		return terms;
	}


	public async Task<ClientDataAccess.CreateTermReturn> CreateTerm_Async(
				ClientDataAccess.CreateTermParams parameters ) {
		IEnumerable<TermEntry> terms = await this.GetTermsByCriteria_Async(
			new ClientDataAccess.GetTermsByCriteriaParams(
				termPattern: parameters.TermPattern,
				context: parameters.Context
			)
		);
		if( terms.Count() > 0 ) {
			return new ClientDataAccess.CreateTermReturn( false, terms.First() );
		}

        long id = this.CurrentTermId++;
        var newTerm = new TermEntry(
			id: id,
			term: parameters.TermPattern,
			context: parameters.Context,
			alias: parameters.Alias
		);

		this.Terms[id] = newTerm;

        return new ClientDataAccess.CreateTermReturn( true, newTerm );
	}
}
