using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject(
            long id,
            string name,
            string? description,
            List<IdDataObject<UserContextTermEntryObject>> entries ) {
    public long Id { get; set; } = id;

    public string Name { get; set; } = name;
    
    public string? Description { get; set; } = description;

    public List<IdDataObject<UserContextTermEntryObject>> Entries { get; set; } = entries;



    public (bool allEntriesAvailable, IEnumerable<UserContextTermEntryObject> availableEntries) GetRequiredEntries() {
        if( this.Entries.Any(e => e.Data is null) ) {
            return (false, []);
        }
        return (true, this.Entries
            .Select( e => e.Data! )
            .Where( e => e.IsRequired )
        );
    }

    public (bool allEntriesAvailable, IEnumerable<UserContextTermEntryObject> availableEntries) GetOptionalEntries() {
        if( this.Entries.Any(e => e.Data is null) ) {
            return (false, []);
        }
        return (true, this.Entries
            .Select( e => e.Data! )
            .Where( e => !e.IsRequired )
        );
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
