using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextEntryObject {
    public class DatabaseEntry {
        public long TermId = default;
        public double Priority = default;
        public bool IsRequired = default;
    }
}
