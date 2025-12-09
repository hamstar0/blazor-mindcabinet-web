using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class Prototype {
        public string? Name;
        public string? Description;

        public List<UserContextEntryObject.DatabaseEntry> Entries = new List<UserContextEntryObject.DatabaseEntry>();


        public bool IsValid() {
            return !string.IsNullOrEmpty(this.Name)
                && this.Entries.Any();
        }

        public UserContextObject.DatabaseEntry CreateDatabaseEntry() {
            if( !this.IsValid() ) {
                throw new InvalidOperationException("Cannot create database entry from invalid prototype.");
            }
            
            return new UserContextObject.DatabaseEntry {
                Name = this.Name ?? "",
                Description = this.Description,
                Entries = this.Entries
            };
        }
    }
}
