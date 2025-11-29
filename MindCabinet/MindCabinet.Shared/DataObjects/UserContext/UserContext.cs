using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContext( long id, string Name, List<UserContextEntry> entries ) {
    public long Id { get; set; } = id;

    public string Name { get; set; } = Name;

    public List<UserContextEntry> Entries { get; set; } = entries;



    public override string ToString() {
        Func<UserContextEntry, string> entry = (e) => $"{e.Term} - {e.Priority}";
        IEnumerable<string> entries = this.Entries.Select( entry );
		return $"{this.Name}: {string.Join( ", ", entries )}";
    }
}
