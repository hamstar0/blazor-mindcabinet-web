using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class UserContextEntryDbData {
        public long TermId = default;
        public double Priority = default;
    }

    public class UserContextWithTermEntries_DbData {
        public long ContextId = default;
        public long SimpleUserId = default;
        public string Name = "";
        public string? Description;

        public IEnumerable<UserContextEntryDbData> Entries = default!;
    }
}
