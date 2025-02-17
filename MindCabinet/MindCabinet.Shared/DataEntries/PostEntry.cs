using System.Text.Json.Serialization;


namespace MindCabinet.Shared.DataEntries;


public class PostEntry : IEquatable<PostEntry> {
	public long? Id { get; private set; } = null;

	[JsonIgnore]
	public bool IsAssignedId { get; private set; } = false;


	public DateTime Created { get; set; }

    public string Body { get; set; }

    public IList<TermEntry> Tags { get; set; }



    public PostEntry() {
		this.Body = string.Empty;
		this.Tags = new List<TermEntry>();
	}

	public PostEntry( DateTime created, string body, IList<TermEntry> tags ) {
		this.Created = created;
		this.Body = body;
		this.Tags = tags;
	}

	public PostEntry( long id, DateTime created, string body, IList<TermEntry> tags ) {
        this.Id = id;
        this.Created = created;
        this.Body = body;
		this.Tags = tags;
	}


	public bool Equals( PostEntry? other ) {
		if( other is null ) { return false; }
		if( this == other ) { return true; }

		if( this.Id != other.Id ) { return false; }
		if( this.ContentEquals(other, true) ) { return false; }
		return true;
	}

	public bool ContentEquals( PostEntry other, bool includeCreateDate ) {
        if( includeCreateDate && this.Created != other.Created ) { return false; }
        if( this.Body != other.Body ) { return false; }
		if( this.Tags.Count != other.Tags.Count ) { return false; }
		if( !this.Tags.All( t => other.Tags.Any(t2 => t2.Equals(t))) ) { return false; }
		return true;
	}


	public bool Test( string bodyPattern, ISet<TermEntry> tags ) {
		if( !string.IsNullOrEmpty(bodyPattern) && !this.Body.Contains(bodyPattern) ) {
			return false;
		}

		if( tags.Count() > 0 ) {
			if( !tags.All(t => this.Tags.Any(t2 => t2.Equals(t))) ) {
				return false;
			}
		}

		return true;
	}
}
