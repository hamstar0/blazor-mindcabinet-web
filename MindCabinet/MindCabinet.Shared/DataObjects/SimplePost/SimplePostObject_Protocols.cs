using MindCabinet.Shared.DataObjects.Term;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class SimplePostObject : IEquatable<SimplePostObject> {
    public class Raw : IRawDataObject {
        public long Id { get; set; }

        public DateTime Created { get; set; }

        public string Body { get; set; } = "";

        public long[] TagsTermIdSet { get; set; } = [];


        
        public async Task<SimplePostObject> CreateDataObject_Async(
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
