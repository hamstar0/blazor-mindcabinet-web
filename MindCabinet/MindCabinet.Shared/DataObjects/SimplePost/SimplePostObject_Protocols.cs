using MindCabinet.Shared.DataObjects.Term;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class SimplePostObject : IEquatable<SimplePostObject> {
    public class DatabaseEntry {
        public long Id;
        public DateTime Created;
        public string Body = "";
        public long TermSetId;


        
        public async Task<SimplePostObject> CreateSimplePost_Async(
                    Func<long, Task<TermSetObject>> termSetFactory ) {
            return new SimplePostObject(
                id: this.Id,
                created: this.Created,
                body: this.Body,
                tags: await termSetFactory(this.TermSetId)
            );
        }
    }
}
