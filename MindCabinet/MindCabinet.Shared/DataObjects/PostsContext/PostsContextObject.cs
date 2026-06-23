using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.PostsContext;


public enum PostsContextId : long { }



public partial class PostsContextObject : IDataObject { //IHasId<PostsContextId>
    public PostsContextId Id { get; }

    public string Name { get; }
    
    public string? Description { get; }
    
    public SimpleUserId Owner { get; }

    public PostsContextTermEntryObject[] Entries { get; }



    public PostsContextObject(
            PostsContextId id,
            string name,
            string? description,
            SimpleUserId owner,
            PostsContextTermEntryObject[] entries ) {
        if( id == 0 ) {
            throw new ArgumentException( $"Id cannot be 0 in {nameof(PostsContextObject)}." );
        }
        if( owner == 0 ) {
            throw new ArgumentException( $"owner cannot be 0 in {nameof(PostsContextObject)}." );
        }

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Owner = owner;
        this.Entries = entries;
    }


    public IEnumerable<PostsContextTermEntryObject> GetRequiredEntries() {
        return this.Entries
            .Where( e => e.IsRequired );
    }

    public IEnumerable<PostsContextTermEntryObject> GetOptionalEntries() {
        return this.Entries
            .Where( e => !e.IsRequired );
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
