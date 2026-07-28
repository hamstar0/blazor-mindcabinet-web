using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.PostsQuery;


public enum PostsQueryId : long { }



public partial class PostsQueryObject : IDataObject { //IHasId<PostsQueryId>
    public PostsQueryId Id { get; }

    public string Name { get; }
    
    public string? Description { get; }
    
    public SimpleUserId Owner { get; }

    public PostsQueryTermEntryObject[] Entries { get; }



    public PostsQueryObject(
            PostsQueryId id,
            string name,
            string? description,
            SimpleUserId owner,
            PostsQueryTermEntryObject[] entries ) {
        if( id == 0 ) {
            throw new ArgumentException( $"Id cannot be 0 in {nameof(PostsQueryObject)}." );
        }
        if( owner == 0 ) {
            throw new ArgumentException( $"owner cannot be 0 in {nameof(PostsQueryObject)}." );
        }

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Owner = owner;
        this.Entries = entries;
    }


    public IEnumerable<PostsQueryTermEntryObject> GetRequiredEntries() {
        return this.Entries
            .Where( e => e.IsRequired );
    }

    public IEnumerable<PostsQueryTermEntryObject> GetOptionalEntries() {
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
