using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsContext;


public partial class PostsContextObject {
    public class Prototype {
        public static bool ValidateEntries(
                    IEnumerable<PostsContextTermEntryObject.Prototype> entries,
                    bool ignorePostsContextId ) {
            return entries.Count() > 0
                && entries.All( e => e.IsValid(ignorePostsContextId) );
        }



        public PostsContextId? Id { get; set; }

        public string? Name { get; set; }
        
        public string? Description { get; set; }
        
        public SimpleUserId? Owner { get; set; }

        public PostsContextTermEntryObject.Prototype[] Entries { get; set; } = [];



        public bool IsValid( bool includingId ) {
            if( includingId && !PostsContextObject.ValidateId(this.Id ?? 0) ) {
                return false;
            }
            if( !PostsContextObject.ValidateName(this.Name ?? "") ) {
                return false;
            }
            if( this.Owner is null || this.Owner == 0 ) {
                return false;
            }
            if( PostsContextObject.Prototype.ValidateEntries(this.Entries, !includingId) == false ) {
                return false;
            }
            
            return true;
        }

        public PostsContextObject.Raw ToRaw( bool validateId ) {
            if( !this.IsValid(validateId) ) {
                throw new InvalidOperationException("Cannot create raw entry from invalid prototype.");
            }

            foreach( PostsContextTermEntryObject.Prototype entry in this.Entries ) {
                if( entry.PostsContextId != this.Id ) {
                    throw new InvalidOperationException("All entries must have the same PostsContextId as the prototype.");
                }
            }
            
            return PostsContextObject.CreateRaw(
                id: this.Id ?? throw new InvalidOperationException("Cannot create raw entry from prototype with null Id."),
                name: this.Name ?? "",
                description: this.Description,
                owner: this.Owner!.Value,
                entries: this.Entries.Select( e => e.ToRaw(false, true) ).ToArray()
            );
        }
    }
    

    public Prototype ToPrototype() {
        return new Prototype {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            Owner = this.Owner,
            Entries = this.Entries
                .Select( e => e.ToPrototype(this.Id) )
                .ToArray()
        };
    }
}
