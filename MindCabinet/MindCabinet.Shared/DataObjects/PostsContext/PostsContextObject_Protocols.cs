using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsContext;


public partial class PostsContextObject {
    public static Raw CreateRaw(
            PostsContextId id,
            string name,
            string? description,
            SimpleUserId owner,
            PostsContextTermEntryObject.Raw[] entries ) {
        return new Raw {
            Id = id,
            Name = name,
            Description = description,
            Owner = owner,
            Entries = entries
        };
    }

    public class Raw : IRawDataObject { //IHasId<PostsContextId>
        public PostsContextId Id { get; set; } = default;

        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public SimpleUserId Owner { get; set; }

        public PostsContextTermEntryObject.Raw[] Entries { get; set; } = [];



        public bool IsValid( bool ignoreId ) {
            if( !ignoreId && this.Id == default ) {
                return false;
            }
            if( !PostsContextObject.ValidateName(this.Name) ) {
                return false;
            }
            if( this.Owner == 0 ) {
                return false;
            }
            if( this.Entries.Any(e => !e.IsValid(ignoreId)) ) {
                return false;
            }
            return true;
        }

        public async Task<PostsContextObject> ToDataObject_Async(
                    Func<PostsContextTermEntryObject.Raw[],
                    Task<PostsContextTermEntryObject[]>> ctxTermsFactory ) {
            PostsContextTermEntryObject[] entries = await ctxTermsFactory( this.Entries );

            return new PostsContextObject(
                id: this.Id,
                name: this.Name,
                description: this.Description,
                owner: this.Owner,
                entries: entries
            );
        }

        public Prototype ToPrototype() {
            return new Prototype {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                Owner = this.Owner,
                Entries = this.Entries
                    .Select( e => e.ToPrototype() )
                    .ToArray()
            };
        }

        
        public IEnumerable<PostsContextTermEntryObject.Raw> GetRequiredEntries() {
            return this.Entries
                .Where( e => e.IsRequired );
        }

        public IEnumerable<PostsContextTermEntryObject.Raw> GetOptionalEntries() {
            return this.Entries
                .Where( e => !e.IsRequired );
        }
    }
    

    public PostsContextObject.Raw ToRaw() {
        return PostsContextObject.CreateRaw(
            id: this.Id,
            name: this.Name ?? "",
            description: this.Description,
            owner: this.Owner,
            entries: this.Entries.Select( e => e.ToRaw(this.Id) ).ToArray()
        );
    }
}
