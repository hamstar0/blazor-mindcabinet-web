using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsContext;


public partial class PostsContextObject {
    public static Raw CreateRaw(
            PostsContextId id,
            string name,
            string? description,
            PostsContextTermEntryObject.Raw[] entries ) {
        return new Raw {
            Id = id,
            Name = name,
            Description = description,
            Entries = entries
        };
    }

    public class Raw : IRawDataObject, IHasId<PostsContextId> {
        public PostsContextId Id { get; set; } = default;

        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public PostsContextTermEntryObject.Raw[] Entries { get; set; } = [];


        public async Task<PostsContextObject> ToDataObject_Async(
                    Func<PostsContextTermEntryObject.Raw[], Task<PostsContextTermEntryObject[]>> ctxTermsFactory ) {
            PostsContextTermEntryObject[] entries = await ctxTermsFactory( this.Entries );

            return new PostsContextObject(
                id: this.Id,
                name: this.Name,
                description: this.Description,
                entries: entries
            );
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
            entries: this.Entries.Select( e => e.ToRaw(this.Id) ).ToArray()
        );
    }
}
