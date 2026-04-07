//global using SimplePostId = System.Int64;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Data;
using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataObjects;


public enum SimplePostId : long { }



public partial class SimplePostObject : IEquatable<SimplePostObject>, IDataObject {
	public SimplePostId Id { get; }


	public DateTime Created { get; }

	public DateTime Modified { get; }

	public SimpleUserId SimpleUserId { get; }

    public string Body { get; }

    public SortedSet<TermObject> Tags { get; }



	public SimplePostObject( SimplePostId id, DateTime created, DateTime modified, SimpleUserId simpleUserId, string body, SortedSet<TermObject> tags ) {
		if( id == 0 ) {
			throw new ArgumentException( "Id cannot be 0 in SimplePostObject." );
		}

        this.Id = id;
        this.Created = created;
        this.Modified = modified;
        this.SimpleUserId = simpleUserId;
        this.Body = body;
		this.Tags = tags;
	}


	public bool Equals( SimplePostObject? other ) {
		if( other is null ) { return false; }
		if( this == other ) { return true; }

		if( this.Id != other.Id ) { return false; }
		if( this.ContentEquals(other, true) ) { return false; }
		return true;
	}

	public bool ContentEquals( SimplePostObject other, bool includeDates ) {
        if( includeDates ) {
			if( this.Created != other.Created ) { return false; }
			if( this.Modified != other.Modified ) { return false; }
        }
        if( this.Body != other.Body ) { return false; }
		if( this.Tags.Count() != other.Tags.Count() ) { return false; }
		if( this.Tags is not null ) {
			if( !this.Tags.All( kv => other.Tags.Any(kv2 => kv2.Id == kv.Id) ) ) {
				return false;
			}
		}
		return true;
	}


	public bool Test( string bodyPattern, IEnumerable<TermId> tagIds ) {
		if( !string.IsNullOrEmpty(bodyPattern) ) {
			 if( !this.Body.Contains(bodyPattern) ) {	// TODO
				return false;
			}
		}

		if( tagIds.Count() > 0 ) {
			if( this.Tags is null || !tagIds.All(id => this.Tags.Any(t => t.Id == id)) ) {
				return false;
			}
		}

		return true;
	}
}
