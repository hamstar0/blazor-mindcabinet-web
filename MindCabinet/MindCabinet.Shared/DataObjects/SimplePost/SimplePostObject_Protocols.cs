using MindCabinet.Shared.DataObjects.Term;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class SimplePostObject : IEquatable<SimplePostObject> {
    public class DatabaseEntry {
        public long Id;

        public DateTime Created;

        public string Body = "";

        public long[] TagsTermIdSet = [];


        
        public async Task<SimplePostObject> CreateSimplePost_Async(
                    Func<long[], Task<TermObject[]>> termsFactory ) {
            return new SimplePostObject(
                id: this.Id,
                created: this.Created,
                body: this.Body,
                tags: new SortedSet<TermObject>( await termsFactory(this.TagsTermIdSet) )
            );
        }
    }
}
