using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsContext;


public partial class PostsContextTermEntryObject {
    public static Raw CreateRaw(
            PostsContextId postsContextId,
            TermId termId,
            double priority,
            bool isRequired ) {
        return new Raw {
            PostsContextId = postsContextId,
            TermId = termId,
            Priority = priority,
            IsRequired = isRequired
        };
    }

    public class Raw : IRawDataObject {
        public PostsContextId PostsContextId { get; set; } = default;

        public TermId TermId { get; set; } = default;

        public double Priority { get; set; } = default;

        public bool IsRequired { get; set; } = default;


		public async Task<PostsContextTermEntryObject> CreateDataObject_Async(
                    Func<TermId, Task<TermObject>> termFactory ) {
            return new PostsContextTermEntryObject(
                term: await termFactory( this.TermId ),
                priority: this.Priority,
                isRequired: this.IsRequired
            );
		}
	}


    public Raw ToRaw( PostsContextId contextId ) {
        return new Raw {
            PostsContextId = contextId,
            TermId = this.Term.Id,
            Priority = this.Priority,
            IsRequired = this.IsRequired
        };
    }
}
