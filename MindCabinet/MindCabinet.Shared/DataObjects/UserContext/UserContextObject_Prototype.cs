using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class Prototype {
        public string? Name;
        public string? Description;

        public IEnumerable<UserContextEntryDbData> Entries = new List<UserContextEntryDbData>();
    }
}
