using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class DatabaseEntry {
        public long ContextId = default;
        public string Name = "";
        public string? Description;

        public IEnumerable<UserContextTermEntryObject.DatabaseEntry> Entries = default!;
    }
}
