using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsQuery;


public partial class PostsQueryObject {
    public const int MinNameLength = 2;
    
    public const int MaxNameLength = 64;

    public static ISet<char> AllowedSpecialCharacters = new HashSet<char>(
        ":;,./<>?|[]{}!@#$%^&*-_=+`~".ToCharArray()    // excludes: '\"()\\
    );



    public static bool Validate( PostsQueryId id, string name, SimpleUserId owner, PostsQueryTermEntryObject[] entries ) {
        if( !PostsQueryObject.ValidateId(id) ) {
            return false;
        }
        if( !PostsQueryObject.ValidateName(name) ) {
            return false;
        }
        if( !PostsQueryObject.ValidateOwner(owner) ) {
            return false;
        }
        if( !PostsQueryObject.ValidateEntries(entries) ) {
            return false;
        }
        return true;
    }

    public static bool ValidateId( PostsQueryId id ) {
        return id != 0;
    }

    public static bool ValidateName( string name ) {
        if( string.IsNullOrWhiteSpace(name) ) {
            return false;
        }
        if( name.Length < MinNameLength ) {
            return false;
        }
        if( name.Length > MaxNameLength ) {
            return false;
        }
        
        int consecWhites = 0;
        bool keyboardOnly = name.All( c => {
            if( char.IsLetterOrDigit(c) || PostsQueryObject.AllowedSpecialCharacters.Contains(c) ) {
                consecWhites = 0;
                return true;
            }
            if( char.IsWhiteSpace(c) && consecWhites < 2 ) {
                consecWhites++;
                return true;
            }
            return false;
        } );

        return true;
    }

    public static bool ValidateOwner( SimpleUserId id ) {
        return id != 0;
    }

    public static bool ValidateEntries( IEnumerable<PostsQueryTermEntryObject> entries ) {
        return entries.Count() > 0
            && entries.All( e => e.IsValid() );
    }

    
    public enum MatchResult {
        Unknown = -1,
        Match = 0,
        IdMismatch = 1,
        NameMismatch = 2,
        DescriptionMismatch = 3,
        OwnerMismatch = 4,
        EntriesCountMismatch = 5,
        EntriesMismatchId = 6,
        EntriesMismatchPriority = 7,
        EntriesMismatchIsRequired = 8
    }

    public MatchResult Matches(
                PostsQueryId? id,
                string name,
                string? description,
                SimpleUserId owner,
                PostsQueryTermEntryObject[] entries,
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
        if( owner != this.Owner ) {
            return MatchResult.OwnerMismatch;
        }
        if( entries.Length != this.Entries.Length ) {
            return MatchResult.EntriesCountMismatch;
        }
        
        for( int i = 0; i < entries.Length; i++ ) {
            PostsQueryTermEntryObject entryA = entries[i];
            PostsQueryTermEntryObject entryB = this.Entries[i];

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

    public MatchResult Matches( PostsQueryObject other, bool ignoreId ) {
        return this.Matches(
            id: ignoreId ? null : other.Id,
            name: other.Name,
            description: other.Description,
            owner: other.Owner,
            entries: other.Entries
        );
    }


    public bool IsValid() {
        return PostsQueryObject.Validate( this.Id, this.Name, this.Owner, this.Entries );
    }
}
