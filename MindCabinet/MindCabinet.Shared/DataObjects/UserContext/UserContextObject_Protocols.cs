using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class Raw : IRawDataObject {
        public long Id { get; set; } = default;

        public long SimpleUserId { get; set; } = default;

        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public UserContextTermEntryObject.Raw[] Entries { get; set; } = [];



        public async Task<UserContextObject> CreateDataObject_Async(
                    Func<UserContextTermEntryObject.Raw[], Task<UserContextTermEntryObject[]>> termsFactory ) {
            UserContextTermEntryObject[] entries = await termsFactory( this.Entries );

            return new UserContextObject(
                id: this.Id,
                name: this.Name,
                description: this.Description,
                entries: entries
            );
        }

        
        public IEnumerable<UserContextTermEntryObject.Raw> GetRequiredEntries() {
            return this.Entries
                .Where( e => e.IsRequired );
        }

        public IEnumerable<UserContextTermEntryObject.Raw> GetOptionalEntries() {
            return this.Entries
                .Where( e => !e.IsRequired );
        }
    }
}
