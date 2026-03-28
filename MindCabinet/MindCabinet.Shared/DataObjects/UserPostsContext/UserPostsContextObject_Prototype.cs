using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserPostsContext;


public partial class UserPostsContextObject {
    public class Prototype {
        public UserPostsContextId? Id { get; set; }

        public string? Name { get; set; }
        
        public string? Description { get; set; }

        public UserPostsContextTermEntryObject.Raw[] Entries { get; set; } = [];



        public bool Matches( UserPostsContextObject other ) {
            if( this.Id != other.Id ) {
                return false;
            }
            if( this.Name != other.Name ) {
                return false;
            }
            if( this.Description != other.Description ) {
                return false;
            }
            if( this.Entries.Length != other.Entries.Length ) {
                return false;
            }
            
            for( int i = 0; i < this.Entries.Length; i++ ) {
                UserPostsContextTermEntryObject.Raw entryA = this.Entries[i];
                UserPostsContextTermEntryObject entryB = other.Entries[i];

                if( entryA.TermId != entryB.Term.Id
                        || entryA.Priority != entryB.Priority
                        || entryA.IsRequired != entryB.IsRequired ) {
                    return false;
                }
            }

            return true;
        }

        public bool IsValid( bool includingId ) {
            if( includingId ) {
                if( this.Id is null || this.Id == 0 ) {
                    return false;
                }
            }
            return !string.IsNullOrEmpty(this.Name)
                && this.Entries.Length > 0;
        }

        public UserPostsContextObject.Raw ToRaw( bool validateId ) {
            if( !this.IsValid(validateId) ) {
                throw new InvalidOperationException("Cannot create raw entry from invalid prototype.");
            }

            foreach( UserPostsContextTermEntryObject.Raw entry in this.Entries ) {
                if( entry.UserPostsContextId != this.Id ) {
                    throw new InvalidOperationException("All entries must have the same UserPostsContextId as the prototype.");
                }
            }
            
            return UserPostsContextObject.CreateRaw(
                id: this.Id ?? throw new InvalidOperationException("Cannot create raw entry from prototype with null Id."),
                name: this.Name ?? "",
                description: this.Description,
                entries: this.Entries
            );
        }
    }
    

    public Prototype ToPrototype() {
        return new Prototype {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            Entries = this.Entries
                .Select( e => e.ToRaw(this.Id) )
                .ToArray()
        };
    }
}
