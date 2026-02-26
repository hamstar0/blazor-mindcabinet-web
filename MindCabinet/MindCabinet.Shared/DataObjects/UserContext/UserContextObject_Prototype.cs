using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class Prototype {
        public string? Name;
        public string? Description;

        public List<UserContextTermEntryObject.DatabaseEntry> Entries = new List<UserContextTermEntryObject.DatabaseEntry>();



        public bool Matches( UserContextObject other ) {
            if( other.Entries.Any(e => e.Data is null) ) {
                throw new DataException( "Could not compare prototype to a context with missing entry data." );
            }

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
                UserContextTermEntryObject.DatabaseEntry entryA = this.Entries[i];
                IdDataObject<UserContextTermEntryObject> entryB = other.Entries[i];

                if( entryA.TermId != entryB.Data!.Term.Id
                    || entryA.Priority != entryB.Data!.Priority
                    || entryA.IsRequired != entryB.Data!.IsRequired ) {
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
