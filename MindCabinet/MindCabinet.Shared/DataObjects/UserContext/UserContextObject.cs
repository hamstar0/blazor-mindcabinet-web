using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.UserContext;


public enum UserContextId : long { }



public partial class UserContextObject : IDataObject {
    public UserContextId Id { get; }

    public string Name { get; }
    
    public string? Description { get; }

    public UserContextTermEntryObject[] Entries { get; }



    public UserContextObject(
            UserContextId id,
            string name,
            string? description,
            UserContextTermEntryObject[] entries ) {
        if( id == 0 ) {
            throw new ArgumentException( "Id cannot be 0 in UserContextObject." );
        }

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Entries = entries;
    }


    public IEnumerable<UserContextTermEntryObject> GetRequiredEntries() {
        return this.Entries
            .Where( e => e.IsRequired );
    }

    public IEnumerable<UserContextTermEntryObject> GetOptionalEntries() {
        return this.Entries
            .Where( e => !e.IsRequired );
    }
    

    public UserContextObject.Raw ToRaw() {
        return new UserContextObject.Raw {
            Id = this.Id,
            Name = this.Name ?? "",
            Description = this.Description,
            Entries = this.Entries.Select( e => e.ToRaw(this.Id) ).ToArray()
        };
    }


    public override string ToString() {
		return $"{this.Name}: {string.Join(", ", this.Entries.Select(e => e.ToString()))}";
    }

    public string ToFullString( bool includeId ) {
		string output = this.Name;
        if( includeId ) {
            output += $" (Id: {this.Id})";
        }
        output += $": {string.Join(", ", this.Entries.Select(e => e.ToString()))}";

        return output;
    }
}
