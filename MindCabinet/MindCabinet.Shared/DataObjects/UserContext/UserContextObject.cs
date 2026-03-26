using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.UserPostsContext;


public enum UserPostsContextId : long { }



public partial class UserPostsContextObject : IDataObject {
    public UserPostsContextId Id { get; }

    public string Name { get; }
    
    public string? Description { get; }

    public UserPostsContextTermEntryObject[] Entries { get; }



    public UserPostsContextObject(
            UserPostsContextId id,
            string name,
            string? description,
            UserPostsContextTermEntryObject[] entries ) {
        if( id == 0 ) {
            throw new ArgumentException( $"Id cannot be 0 in {nameof(UserPostsContextObject)}." );
        }

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Entries = entries;
    }


    public IEnumerable<UserPostsContextTermEntryObject> GetRequiredEntries() {
        return this.Entries
            .Where( e => e.IsRequired );
    }

    public IEnumerable<UserPostsContextTermEntryObject> GetOptionalEntries() {
        return this.Entries
            .Where( e => !e.IsRequired );
    }
    

    public UserPostsContextObject.Raw ToRaw() {
        return new UserPostsContextObject.Raw {
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
