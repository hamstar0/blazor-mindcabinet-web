using MindCabinet.Shared.DataObjects.Term;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public partial class SimplePostObject {
    public static Raw CreateRaw(
            SimplePostId id,
            DateTime created,
            DateTime modified,
            SimpleUserId simpleUserId,
            string body,
            TermId[] tagsTermIdSet ) {
        return new Raw {
            Id = id,
            Created = created,
            Modified = modified,
            SimpleUserId = simpleUserId,
            Body = body,
            TagsTermIdSet = tagsTermIdSet
        };
    }

    public class Raw : IRawDataObject {
        public SimplePostId Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public SimpleUserId SimpleUserId { get; set; }

        public string Body { get; set; } = "";

        public TermId[] TagsTermIdSet { get; set; } = [];



        public async Task<SimplePostObject> ToDataObject_Async(
                    Func<TermId[], Task<TermObject[]>> termsFactory ) {
            return new SimplePostObject(
                id: this.Id,
                created: this.Created,
                modified: this.Modified,
                simpleUserId: this.SimpleUserId,
                body: this.Body,
                tags: new SortedSet<TermObject>( await termsFactory(this.TagsTermIdSet) )
            );
        }
    }
}
