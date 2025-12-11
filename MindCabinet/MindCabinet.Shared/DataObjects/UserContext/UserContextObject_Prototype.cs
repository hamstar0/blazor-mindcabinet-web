using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class Prototype {
        public string? Name;
        public string? Description;

        public List<UserContextEntryObject.DatabaseEntry> Entries = new List<UserContextEntryObject.DatabaseEntry>();



        public bool Matches( UserContextObject other ) {
            if( this.Name != other.Name ) {
                return false;
            }
            if( this.Description != other.Description ) {
                return false;
            }
            if( this.Entries.Count != other.Entries.Count ) {
                return false;
            }

            for( int i = 0; i < this.Entries.Count; i++ ) {
                UserContextEntryObject.DatabaseEntry entryA = this.Entries[i];
                UserContextEntryObject entryB = other.Entries[i];

                if( entryA.TermId != entryB.Term.Id
                    || entryA.Priority != entryB.Priority
                    || entryA.IsRequired != entryB.IsRequired ) {
                    return false;
                }
            }

            return true;
        }

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
