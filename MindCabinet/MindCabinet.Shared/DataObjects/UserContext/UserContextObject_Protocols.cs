using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class DatabaseEntry {
        public long Id = default;
        
        public string Name = "";

        public string? Description;

        public IEnumerable<UserContextTermEntryObject.DatabaseEntry> Entries = default!;



        public async Task<UserContextObject?> CreateUserContextObject_Async(
                    Func<UserContextTermEntryObject.DatabaseEntry, Task<UserContextTermEntryObject?>> termFactory ) {
            IEnumerable<Task<UserContextTermEntryObject?>> entriesTasks = this.Entries.Select( async e => await termFactory(e) );
            IEnumerable<UserContextTermEntryObject?> entries = await Task.WhenAll( entriesTasks );

            if( entries.Any(e => e is null) ) {
                return null;
            }

            return new UserContextObject(
                id: this.Id,
                name: this.Name,
                description: this.Description,
                entries: entries.Select( e => e! ).ToList()
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
