using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject(
            long id,
            string name,
            string? description,
            UserContextTermEntryObject[] entries ) {
    public long Id { get; } = id;

    public string Name { get; } = name;
    
    public string? Description { get; } = description;

    public UserContextTermEntryObject[] Entries { get; } = entries;



    public IEnumerable<UserContextTermEntryObject> GetRequiredEntries() {
        return this.Entries
            .Where( e => e.IsRequired );
    }

    public IEnumerable<UserContextTermEntryObject> GetOptionalEntries() {
        return this.Entries
            .Where( e => !e.IsRequired );
    }
    

    public UserContextObject.Raw ToRaw() {
        return new UserContextObject.Raw {
            Name = this.Name ?? "",
            Description = this.Description,
            Entries = this.Entries.Select( e => e.ToRaw() ).ToArray()
        };
    }


    public override string ToString() {
		return $"{this.Name}: {string.Join(", ", this.Entries.Select(e => e.ToString()))}";
    }

    public string ToFullString( bool includeId ) {
		string output = this.Name;
        if( includeId ) {
            output += $" (Id: {this.Id})";
        }
        output += $": {string.Join(", ", this.Entries.Select(e => e.ToString()))}";

        return output;
    }
}
