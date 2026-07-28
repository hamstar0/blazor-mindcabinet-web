using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsQuery;


public partial class PostsQueryObject {
    public static Raw CreateRaw(
            PostsQueryId id,
            string name,
            string? description,
            SimpleUserId owner,
            PostsQueryTermEntryObject.Raw[] entries ) {
        return new Raw {
            Id = id,
            Name = name,
            Description = description,
            Owner = owner,
            Entries = entries
        };
    }

    public class Raw : IRawDataObject { //IHasId<PostsQueryId>
        public PostsQueryId Id { get; set; } = default;

        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public SimpleUserId Owner { get; set; }

        public PostsQueryTermEntryObject.Raw[] Entries { get; set; } = [];



        public bool IsValid( bool ignoreId ) {
            if( !ignoreId && this.Id == default ) {
                return false;
            }
            if( !PostsQueryObject.ValidateName(this.Name) ) {
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

        public async Task<PostsQueryObject> ToDataObject_Async(
                    Func<PostsQueryTermEntryObject.Raw[],
                    Task<PostsQueryTermEntryObject[]>> queryTermsFactory ) {
            PostsQueryTermEntryObject[] entries = await queryTermsFactory( this.Entries );

            return new PostsQueryObject(
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

        
        public IEnumerable<PostsQueryTermEntryObject.Raw> GetRequiredEntries() {
            return this.Entries
                .Where( e => e.IsRequired );
        }

        public IEnumerable<PostsQueryTermEntryObject.Raw> GetOptionalEntries() {
            return this.Entries
                .Where( e => !e.IsRequired );
        }
    }
    

    public PostsQueryObject.Raw ToRaw() {
        return PostsQueryObject.CreateRaw(
            id: this.Id,
            name: this.Name ?? "",
            description: this.Description,
            owner: this.Owner,
            entries: this.Entries.Select( e => e.ToRaw(this.Id) ).ToArray()
        );
    }
}
