using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsQuery;


public partial class PostsQueryTermEntryObject {
    public class Prototype {
        public PostsQueryId? PostsQueryId { get; set; }

        public TermId? TermId { get; set; }

        public double? Priority { get; set; }

        public bool? IsRequired { get; set; }



        public bool IsValid( bool includePostsQueryId ) {
            if( includePostsQueryId && (this.PostsQueryId == null || this.PostsQueryId == default) ) {
                return false;
            }
            if( this.TermId == null || this.TermId == default ) {
                return false;
            }
            if( this.Priority == null ) {
                return false;
            }
            if( this.IsRequired == null ) {
                return false;
            }
            return true;
        }


        public enum MatchResult {
            Unknown = -1,
            Match = 0,
            PostsQueryIdMismatch = 1,
            TermMismatch = 2,
            PriorityMismatch = 3,
            IsRequiredMismatch = 4,
        }
        public MatchResult Matches( PostsQueryTermEntryObject other ) {
            // if( this.PostsQueryId != other.PostsQueryId ) {
            //     return MatchResult.PostsQueryIdMismatch;
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

        public PostsQueryTermEntryObject.Raw ToRaw( bool validatePostContextId, bool validate ) {
            if( validatePostContextId ) {
                if( this.PostsQueryId is null || this.PostsQueryId == 0 ) {
                    throw new InvalidOperationException("Cannot create raw entry from prototype with null or zero PostsQueryId.");
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

            return PostsQueryTermEntryObject.CreateRaw(
                postsQueryId: this.PostsQueryId ?? default,
                termId: this.TermId ?? default,
                priority: this.Priority ?? default,
                isRequired: this.IsRequired ?? default
            );
        }
    }
    

    public Prototype ToPrototype( PostsQueryId? postContextId ) {
        return new Prototype {
            PostsQueryId = postContextId,
            TermId = this.Term.Id,
            Priority = this.Priority,
            IsRequired = this.IsRequired
        };
    }
}
