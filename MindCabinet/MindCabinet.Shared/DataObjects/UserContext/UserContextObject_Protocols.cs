using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class DatabaseEntry {
        public long Id = default;
        public string Name = "";
        public string? Description;

        public IEnumerable<UserContextTermEntryObject.DatabaseEntry> Entries = default!;



        public async Task<UserContextObject> CreateUserContextObject_Async(
                    Func<IEnumerable<long>, Task<IEnumerable<IdDataObject<UserContextTermEntryObject>>>> termFactory ) {
            IEnumerable<IdDataObject<UserContextTermEntryObject>> entries = await termFactory(
                this.Entries.Select( e => e.TermId )
            );

            return new UserContextObject(
                id: this.Id,
                name: this.Name,
                description: this.Description,
                entries: entries.ToList()
            );
        }
    }
}
