using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserPostsContext;


public partial class UserPostsContextObject {
    public class Raw : IRawDataObject {
        public UserPostsContextId Id { get; set; } = default;

        public SimpleUserId SimpleUserId { get; set; } = default;

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
}
