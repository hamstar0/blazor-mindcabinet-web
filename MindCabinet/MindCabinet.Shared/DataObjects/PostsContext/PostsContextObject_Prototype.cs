using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsContext;


public partial class PostsContextObject {
    public class Prototype {
        public PostsContextId? Id { get; set; }

        public string? Name { get; set; }
        
        public string? Description { get; set; }

        public PostsContextTermEntryObject.Raw[] Entries { get; set; } = [];



        public bool Matches( PostsContextObject other ) {
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
                PostsContextTermEntryObject.Raw entryA = this.Entries[i];
                PostsContextTermEntryObject entryB = other.Entries[i];

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

        public PostsContextObject.Raw ToRaw( bool validateId ) {
            if( !this.IsValid(validateId) ) {
                throw new InvalidOperationException("Cannot create raw entry from invalid prototype.");
            }

            foreach( PostsContextTermEntryObject.Raw entry in this.Entries ) {
                if( entry.PostsContextId != this.Id ) {
                    throw new InvalidOperationException("All entries must have the same PostsContextId as the prototype.");
                }
            }
            
            return PostsContextObject.CreateRaw(
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
