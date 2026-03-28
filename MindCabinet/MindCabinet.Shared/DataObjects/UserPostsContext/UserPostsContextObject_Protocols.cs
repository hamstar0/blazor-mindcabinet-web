using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserPostsContext;


public partial class UserPostsContextObject {
    public static Raw CreateRaw(
            UserPostsContextId id,
            string name,
            string? description,
            UserPostsContextTermEntryObject.Raw[] entries ) {
        return new Raw {
            Id = id,
            Name = name,
            Description = description,
            Entries = entries
        };
    }

    public class Raw : IRawDataObject {
        public UserPostsContextId Id { get; set; } = default;

        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public UserPostsContextTermEntryObject.Raw[] Entries { get; set; } = [];


        public async Task<UserPostsContextObject> CreateDataObject_Async(
                    Func<UserPostsContextTermEntryObject.Raw[], Task<UserPostsContextTermEntryObject[]>> ctxTermsFactory ) {
            UserPostsContextTermEntryObject[] entries = await ctxTermsFactory( this.Entries );

            return new UserPostsContextObject(
                id: this.Id,
                name: this.Name,
                description: this.Description,
                entries: entries
            );
        }

        
        public IEnumerable<UserPostsContextTermEntryObject.Raw> GetRequiredEntries() {
            return this.Entries
                .Where( e => e.IsRequired );
        }

        public IEnumerable<UserPostsContextTermEntryObject.Raw> GetOptionalEntries() {
            return this.Entries
                .Where( e => !e.IsRequired );
        }
    }
    

    public UserPostsContextObject.Raw ToRaw() {
        return UserPostsContextObject.CreateRaw(
            id: this.Id,
            name: this.Name ?? "",
            description: this.Description,
            entries: this.Entries.Select( e => e.ToRaw(this.Id) ).ToArray()
        );
    }
}
