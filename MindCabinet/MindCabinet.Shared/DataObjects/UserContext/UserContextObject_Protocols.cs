using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class DatabaseEntry {
        public long Id = default;

        public long SimpleUserId = default;
        
        public string Name = "";

        public string? Description;

        public UserContextTermEntryObject.DatabaseEntry[] Entries = [];



        public async Task<UserContextObject> CreateUserContextObject_Async(
                    Func<UserContextTermEntryObject.DatabaseEntry[], Task<UserContextTermEntryObject[]>> termsFactory ) {
            UserContextTermEntryObject[] entries = await termsFactory( this.Entries );

            return new UserContextObject(
                id: this.Id,
                name: this.Name,
                description: this.Description,
                entries: entries
            );
        }

        
        public IEnumerable<UserContextTermEntryObject.DatabaseEntry> GetRequiredEntries() {
            return this.Entries
                .Where( e => e.IsRequired );
        }

        public IEnumerable<UserContextTermEntryObject.DatabaseEntry> GetOptionalEntries() {
            return this.Entries
                .Where( e => !e.IsRequired );
        }
    }
}
