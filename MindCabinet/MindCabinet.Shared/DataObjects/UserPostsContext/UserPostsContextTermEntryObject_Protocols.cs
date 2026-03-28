using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserPostsContext;


public partial class UserPostsContextTermEntryObject {
    public static Raw CreateRaw(
            UserPostsContextId userPostsContextId,
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
        public UserPostsContextId UserPostsContextId { get; set; } = default;

        public TermId TermId { get; set; } = default;

        public double Priority { get; set; } = default;

        public bool IsRequired { get; set; } = default;


		public async Task<UserPostsContextTermEntryObject> CreateDataObject_Async(
                    Func<TermId, Task<TermObject>> termFactory ) {
            return new UserPostsContextTermEntryObject(
                term: await termFactory( this.TermId ),
                priority: this.Priority,
                isRequired: this.IsRequired
            );
		}
	}


    public Raw ToRaw( UserPostsContextId contextId ) {
        return new Raw {
            UserPostsContextId = contextId,
            TermId = this.Term.Id,
            Priority = this.Priority,
            IsRequired = this.IsRequired
        };
    }
}
