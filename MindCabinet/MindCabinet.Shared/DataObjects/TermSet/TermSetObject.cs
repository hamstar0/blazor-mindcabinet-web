using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermSetObject {
    public long Id { get; private set; }

    public SortedSet<TermObject> TermSet { get; private set; }



	public TermSetObject( long id, SortedSet<TermObject> termSet ) {
		this.Id = id;
		this.TermSet = termSet;
    }
}
