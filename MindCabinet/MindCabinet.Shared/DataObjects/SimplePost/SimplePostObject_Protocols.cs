using MindCabinet.Shared.DataObjects.Term;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class SimplePostObject : IEquatable<SimplePostObject> {
    public class DatabaseEntry {
        public long Id;
        public DateTime Created;
        public string Body = "";
        public long TermSetId;
    }
}
