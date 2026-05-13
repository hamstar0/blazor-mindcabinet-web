using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsContext;


public partial class PostsContextObject {
    public const int MaxNameLength = 64;



    public static bool ValidateName( string name ) {
        if( !string.IsNullOrWhiteSpace(name) ) {
            return false;
        }
        if( name.Length > MaxNameLength ) {
            return false;
        }
        return true;
    }

    
    public enum MatchResult {
        Unknown = -1,
        Match = 0,
        IdMismatch = 1,
        NameMismatch = 2,
        DescriptionMismatch = 3,
        EntriesCountMismatch = 4,
        EntriesMismatchId = 5,
        EntriesMismatchPriority = 6,
        EntriesMismatchIsRequired = 7
    }

    public MatchResult Matches(
                PostsContextId? id,
                string name,
                string? description,
                PostsContextTermEntryObject[] entries,
                bool ignoreId = false ) {
        if( !ignoreId && id is not null && id != this.Id ) {
            return MatchResult.IdMismatch;
        }
        if( name != this.Name ) {
            return MatchResult.NameMismatch;
        }
        if( description != this.Description ) {
            return MatchResult.DescriptionMismatch;
        }
        if( entries.Length != this.Entries.Length ) {
            return MatchResult.EntriesCountMismatch;
        }
        
        for( int i = 0; i < entries.Length; i++ ) {
            PostsContextTermEntryObject entryA = entries[i];
            PostsContextTermEntryObject entryB = this.Entries[i];

            if( entryA.Term.Id != entryB.Term.Id ) {
                return MatchResult.EntriesMismatchId;
            }
            if( entryA.Priority != entryB.Priority ) {
                return MatchResult.EntriesMismatchPriority;
            }
            if( entryA.IsRequired != entryB.IsRequired ) {
                return MatchResult.EntriesMismatchIsRequired;
            }
        }

        return MatchResult.Match;
    }

    public MatchResult Matches( PostsContextObject other, bool ignoreId ) {
        return this.Matches(
            id: ignoreId ? null : other.Id,
            name: other.Name,
            description: other.Description,
            entries: other.Entries
        );
    }
}
