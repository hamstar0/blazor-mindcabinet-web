using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class DatabaseEntry {
        public long Id = default;
        public string Name = "";
        public string? Description;

        public IEnumerable<UserContextTermEntryObject.DatabaseEntry> Entries = default!;



        public async Task<UserContextObject> CreateUserContextObject_Async( Func<IEnumerable<long>, Task<IEnumerable<TermObject>>> termFactory ) {
            IEnumerable<TermObject> entries = await termFactory(
                this.Entries.Select( e => e.TermId )
            );

            return new UserContextObject(
                id: this.Id,
                name: this.Name,
                description: this.Description,
                entries: entries.Select( t =>
                    new UserContextTermEntryObject(
                        term: t,
                        priority: this.Entries.First( en => en.TermId == t.Id ).Priority,
                        isRequired: this.Entries.First( en => en.TermId == t.Id ).IsRequired
                    ) ).ToList()
            );
        }
    }
}
