using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserPostsContext;


public partial class PostsContextTermEntryObject {
    public static Raw CreateRaw(
            PostsContextId userPostsContextId,
            TermId termId,
            double priority,
            bool isRequired ) {
        return new Raw {
            UserPostsContextId = userPostsContextId,
            TermId = termId,
            Priority = priority,
            IsRequired = isRequired
        };
    }

    public class Raw : IRawDataObject {
        public PostsContextId UserPostsContextId { get; set; } = default;

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
            UserPostsContextId = contextId,
            TermId = this.Term.Id,
            Priority = this.Priority,
            IsRequired = this.IsRequired
        };
    }
}
