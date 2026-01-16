using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject( long id, string name, string? description, List<UserContextTermEntryObject> entries ) {
    public long Id { get; set; } = id;

    public string Name { get; set; } = name;
    
    public string? Description { get; set; } = description;

    public List<UserContextTermEntryObject> Entries { get; set; } = entries;



    public IEnumerable<UserContextTermEntryObject> GetRequiredEntries() {
        return this.Entries.Where( entry => entry.IsRequired );
    }

    public IEnumerable<UserContextTermEntryObject> GetOptionalEntries() {
        return this.Entries.Where( entry => entry.IsRequired );
    }
    

    public override string ToString() {
		return $"{this.Name}: {string.Join(", ", this.Entries)}";
    }
}
