using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class Raw {
        public long Id = default;

        public long SimpleUserId = default;
        
        public string Name = "";

        public string? Description;

        public UserContextTermEntryObject.Raw[] Entries = [];



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
