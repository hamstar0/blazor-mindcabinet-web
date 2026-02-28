using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermSetObject {
    public class DatabaseEntry {
        public long Id = default;
        public long[] TermSet = new long[0];
    }
}
