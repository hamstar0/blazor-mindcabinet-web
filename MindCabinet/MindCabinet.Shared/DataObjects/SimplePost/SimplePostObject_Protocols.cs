using MindCabinet.Shared.DataObjects.Term;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class SimplePostObject : IEquatable<SimplePostObject> {
    public static Raw CreateRaw(
            SimplePostId id,
            DateTime created,
            string body,
            TermId[] tagsTermIdSet ) {
        return new Raw {
            Id = id,
            Created = created,
            Body = body,
            TagsTermIdSet = tagsTermIdSet
        };
    }

    public class Raw : IRawDataObject {
        public SimplePostId Id { get; set; }

        public DateTime Created { get; set; }

        public string Body { get; set; } = "";

        public TermId[] TagsTermIdSet { get; set; } = [];



        public async Task<SimplePostObject> CreateDataObject_Async(
                    Func<TermId[], Task<TermObject[]>> termsFactory ) {
            return new SimplePostObject(
                id: this.Id,
                created: this.Created,
                body: this.Body,
                tags: new SortedSet<TermObject>( await termsFactory(this.TagsTermIdSet) )
            );
        }
    }
}
