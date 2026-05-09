using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsContext;


public partial class PostsContextTermEntryObject {
    public class Prototype {
        public PostsContextId? PostsContextId { get; set; }

        public TermId? TermId { get; set; }

        public double? Priority { get; set; }

        public bool? IsRequired { get; set; }



        public enum MatchResult {
            Unknown = -1,
            Match = 0,
            PostsContextIdMismatch = 1,
            TermMismatch = 2,
            PriorityMismatch = 3,
            IsRequiredMismatch = 4,
        }
        public MatchResult Matches( PostsContextTermEntryObject other ) {
            // if( this.PostsContextId != other.PostsContextId ) {
            //     return MatchResult.PostsContextIdMismatch;
            // }
            if( this.TermId != other.Term.Id ) {
                return MatchResult.TermMismatch;
            }
            if( this.Priority != other.Priority ) {
                return MatchResult.PriorityMismatch;
            }
            if( this.IsRequired != other.IsRequired ) {
                return MatchResult.IsRequiredMismatch;
            }

            return MatchResult.Match;
        }

        public PostsContextTermEntryObject.Raw ToRaw( bool validatePostContextId, bool validate ) {
            if( validatePostContextId ) {
                if( this.PostsContextId is null || this.PostsContextId == 0 ) {
                    throw new InvalidOperationException("Cannot create raw entry from prototype with null or zero PostsContextId.");
                }
            }
            if( validate ) {
                if( this.TermId is null || this.TermId == 0 ) {
                    throw new InvalidOperationException("Cannot create raw entry from prototype with null or zero TermId.");
                }
                if( this.Priority is null ) {
                    throw new InvalidOperationException("Cannot create raw entry from prototype with null Priority.");
                }
                if( this.IsRequired is null ) {
                    throw new InvalidOperationException("Cannot create raw entry from prototype with null IsRequired.");
                }
            }

            return PostsContextTermEntryObject.CreateRaw(
                postsContextId: this.PostsContextId ?? default,
                termId: this.TermId ?? default,
                priority: this.Priority ?? default,
                isRequired: this.IsRequired ?? default
            );
        }
    }
    

    public Prototype ToPrototype( PostsContextId? postContextId ) {
        return new Prototype {
            PostsContextId = postContextId,
            TermId = this.Term.Id,
            Priority = this.Priority,
            IsRequired = this.IsRequired
        };
    }
}
