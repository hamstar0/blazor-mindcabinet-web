using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject(
            long id,
            string name,
            string? description,
            List<UserContextTermEntryObject> entries ) {
    public long Id { get; private set; } = id;

    public string Name { get; private set; } = name;
    
    public string? Description { get; private set; } = description;

    public List<UserContextTermEntryObject> Entries { get; private set; } = entries;



    public IEnumerable<UserContextTermEntryObject> GetRequiredEntries() {
        return this.Entries
            .Select( e => e )
            .Where( e => e.IsRequired );
    }

    public IEnumerable<UserContextTermEntryObject> GetOptionalEntries() {
        return this.Entries
            .Select( e => e )
            .Where( e => !e.IsRequired );
    }
    

    public override string ToString() {
		return $"{this.Name}: {string.Join(", ", this.Entries)}";
    }

    public string ToFullString( bool includeId ) {
		string output = this.Name;
        if( includeId ) {
            output += $" (Id: {this.Id})";
        }
        output += $": {string.Join(", ", this.Entries)}";

        return output;
    }
}
