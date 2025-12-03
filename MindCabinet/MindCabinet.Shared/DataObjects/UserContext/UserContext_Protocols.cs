using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContext {
    public class UserContextWithTermEntries_DbData {
        public class UserContextEntryDbData {
            public long TermId = default;
            public double Priority = default;
        }

        public long ContextId = default;
        public long SimpleUserId = default;
        public string Name = "";

        public IEnumerable<UserContextEntryDbData> Entries = default!;
    }
}
