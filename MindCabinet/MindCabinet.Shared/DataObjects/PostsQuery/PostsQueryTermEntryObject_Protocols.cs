using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsQuery;


public partial class PostsQueryTermEntryObject {
    public static Raw CreateRaw(
            PostsQueryId postsQueryId,
            TermId termId,
            double priority,
            bool isRequired ) {
        return new Raw {
            PostsQueryId = postsQueryId,
            TermId = termId,
            Priority = priority,
            IsRequired = isRequired
        };
    }

    public class Raw : IRawDataObject {
        public PostsQueryId PostsQueryId { get; set; } = default;

        public TermId TermId { get; set; } = default;

        public double Priority { get; set; } = default;

        public bool IsRequired { get; set; } = default;



        public bool IsValid( bool ignorePostsQueryId ) {
            if( !ignorePostsQueryId && this.PostsQueryId == default ) {
                return false;
            }
            if( this.TermId == default ) {
                return false;
            }
            return true;
        }

		public async Task<PostsQueryTermEntryObject> ToDataObject_Async(
                    Func<TermId, Task<TermObject>> termFactory ) {
            return new PostsQueryTermEntryObject(
                term: await termFactory( this.TermId ),
                priority: this.Priority,
                isRequired: this.IsRequired
            );
		}

        public Prototype ToPrototype() {
            return new Prototype {
                PostsQueryId = this.PostsQueryId,
                TermId = this.TermId,
                Priority = this.Priority,
                IsRequired = this.IsRequired
            };
        }
	}


    public Raw ToRaw( PostsQueryId contextId ) {
        return new Raw {
            PostsQueryId = contextId,
            TermId = this.Term.Id,
            Priority = this.Priority,
            IsRequired = this.IsRequired
        };
    }
}
