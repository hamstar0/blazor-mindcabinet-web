using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject( long id, string name, string? description, List<UserContextEntryObject> entries ) {
    public long Id { get; set; } = id;

    public string Name { get; set; } = name;
    
    public string? Description { get; set; } = description;

    public List<UserContextEntryObject> Entries { get; set; } = entries;



    public override string ToString() {
		  return $"{this.Name}: {string.Join(", ", this.Entries)}";
    }
}
