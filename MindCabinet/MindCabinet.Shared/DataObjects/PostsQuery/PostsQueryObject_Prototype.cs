using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsQuery;


public partial class PostsQueryObject {
    public class Prototype {
        public static bool ValidateEntries(
                    IEnumerable<PostsQueryTermEntryObject.Prototype> entries,
                    bool includeId ) {
            return entries.All( e => e.IsValid(includeId) );     //entries.Count() > 0
        }



        public PostsQueryId? Id { get; set; }

        public string? Name { get; set; }
        
        public string? Description { get; set; }
        
        public SimpleUserId? Owner { get; set; }

        public PostsQueryTermEntryObject.Prototype[] Entries { get; set; } = [];



        public bool IsValid( bool includingId ) {
            if( includingId && !PostsQueryObject.ValidateId(this.Id ?? 0) ) {
                return false;
            }
            if( !PostsQueryObject.ValidateName(this.Name ?? "") ) {
                return false;
            }
            if( this.Owner is null || this.Owner == 0 ) {
                return false;
            }
            if( !PostsQueryObject.Prototype.ValidateEntries(this.Entries, includingId) ) {
                return false;
            }
            
            return true;
        }

        public PostsQueryObject.Raw ToRaw( bool validateId ) {
            if( !this.IsValid(validateId) ) {
                throw new InvalidOperationException("Cannot create raw entry from invalid prototype.");
            }
            if( this.Owner is null || this.Owner == 0 ) {
                throw new Exception( "Invalid Owner "+this.Owner );
            }

            foreach( PostsQueryTermEntryObject.Prototype entry in this.Entries ) {
                if( entry.PostsQueryId != this.Id ) {
                    throw new InvalidOperationException("All entries must have the same PostsQueryId as the prototype.");
                }
            }
            
            return PostsQueryObject.CreateRaw(
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
