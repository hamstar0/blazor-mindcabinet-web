using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextObject {
    public class Prototype {
        public string? Name;
        
        public string? Description;

        public UserContextTermEntryObject.Raw[] Entries = [];



        public bool Matches( UserContextObject other ) {
            if( this.Name != other.Name ) {
                return false;
            }
            if( this.Description != other.Description ) {
                return false;
            }
            if( this.Entries.Length != other.Entries.Length ) {
                return false;
            }
            
            for( int i = 0; i < this.Entries.Length; i++ ) {
                UserContextTermEntryObject.Raw entryA = this.Entries[i];
                UserContextTermEntryObject entryB = other.Entries[i];

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

        public UserContextObject.Raw CreateDatabaseEntry() {
            if( !this.IsValid() ) {
                throw new InvalidOperationException("Cannot create database entry from invalid prototype.");
            }
            
            return new UserContextObject.Raw {
                Name = this.Name ?? "",
                Description = this.Description,
                Entries = this.Entries
            };
        }
    }
}
