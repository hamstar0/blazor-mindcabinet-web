using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataEntries;


public class TermEntry : IEquatable<TermEntry>, IComparable, IComparable<TermEntry> {
    public long? Id { get; private set; } = null;

    [JsonIgnore]
	public bool IsAssignedId { get; private set; } = false;

	public string Term { get; set; }

	public TermEntry? Context { get; set; }

    public TermEntry? Alias { get; set; }



    public TermEntry() {
        this.Term = "";
    }

	public TermEntry( string term, TermEntry? context, TermEntry? alias ) {
		this.Term = term;
		this.Context = context;
		this.Alias = alias;
	}

	public TermEntry( long id, string term, TermEntry? context, TermEntry? alias ) {
		this.Id = id;
		this.IsAssignedId = true;
		this.Term = term;
		this.Context = context;
		this.Alias = alias;
    }


    public override int GetHashCode() {
        return HashCode.Combine(
			this.Id,
			this.IsAssignedId,
			this.Term,
            this.Context is null
				? 0 : HashCode.Combine( this.Context.Id, this.Context.IsAssignedId, this.Context.Term ),
            this.Alias is null
				? 0 : HashCode.Combine( this.Alias.Id, this.Alias.IsAssignedId, this.Alias.Term )
		);
    }

    public override bool Equals( object? obj ) {
        return obj is TermEntry
			? this.EqualsTermShallow( obj as TermEntry )
            : base.Equals( obj );
    }

	public bool Equals( TermEntry? other ) {
		return this.EqualsTermShallow( other );
	}

    public bool EqualsTermShallow( TermEntry? other, bool ignoreNullId=true ) {
		if( other is null ) { return false; }
		if( Object.ReferenceEquals(this, other) ) { return true; }

		if( ignoreNullId ) {
            if( this.Id is not null && other.Id is not null && this.Id != other.Id ) {
                return false;
            }
        } else if( this.Id != other.Id ) {
            return false;
        }

		if( !this.Term.Equals(other.Term) ) {
            return false;
        }
		if( !this.Context?.EqualsTermShallow(other.Context, false) ?? other.Context is not null ) {
            return false;
        }
		//if( !this.Alias?.Equals(other.Alias ) ?? other.Alias is not null ) { return false; }

		return true;
    }

    public int CompareTo( object? obj ) {
        if( obj is not TermEntry ) {
            return 1;
        }
        return this.CompareTo( obj as TermEntry );
    }

    public int CompareTo( TermEntry? test ) {
        if( test is null ) {
            return 1;
        }
        return this.ToString().CompareTo( test?.ToString() );   // todo?
        //return this.SafeCompareTo( test, null );
    }

    //private int SafeCompareTo( TermEntry test, ISet<TermEntry>? scanned ) {
    //    if( scanned == null ) {
    //        scanned = new HashSet<TermEntry> { this };
    //    } else if( scanned.Contains( this ) ) {
    //        throw new Exception( "Circular reference" );
    //    } else {
    //        scanned.Add( this );
    //    }
    //
    //    int cmp = this.Term.CompareTo( test.Term );
    //    if( cmp != 0 ) {
    //        return cmp;
    //    }
    //
    //    if( this.Context is not null ) {
    //        if( test.Context is null ) {
    //            return 1;
    //        }
    //        cmp = this.Context.SafeCompareTo( test.Context, scanned );
    //        if( cmp != 0 ) {
    //            return cmp;
    //        }
    //    } else {
    //        if( test.Context is not null ) {
    //            return -1;
    //        }
    //    }
    //
    //    if( this.Alias is not null ) {
    //        if( test.Alias is null ) {
    //            return 1;
    //        }
    //        cmp = this.Alias.SafeCompareTo( test.Alias, scanned );
    //        if( cmp != 0 ) {
    //            return cmp;
    //        }
    //    } else {
    //        if( test.Alias is not null ) {
    //            return -1;
    //        }
    //    }
    //
    //    return 0;
    //}
    //
    //public bool DeepCompare( string term, TermEntry? context ) {
    //    if( this.Term != term ) {
    //        return false;
    //    }
    //
    //    TermEntry? alias = this;
    //    while( alias.Alias is not null ) {
    //        alias = alias.Alias;
    //
    //        if( alias.Term != term ) {
    //            return false;
    //        }
    //    }
    //
    //    if( context is not null ) {
    //        if( this.Context is null ) {
    //            return false;
    //        }
    //        if( this.Context.DeepCompare( context.Term, context.Context ) ) {
    //            return false;
    //        }
    //    } else if( this.Context is not null ) {
    //        return false;
    //    }
    //
    //    return true;
    //}

    public bool DeepTest( string pattern, TermEntry? context ) {
		if( !this.Term.Contains(pattern) ) {
			return false;
		}

		TermEntry? alias = this;
		while( alias.Alias is not null ) {
			alias = alias.Alias;

			if( !alias.Term.Contains(pattern) ) {
				return false;
			}
		}

		if( context is not null ) {
			if( this.Context is null ) {
				return false;
			}
			if( this.Context.DeepTest(context.Term, context.Context) ) {
				return false;
			}
		} else if( this.Context is not null ) {
			return false;
		}

		return true;
	}


    public override string ToString() {
		return this.Context is not null
			? $"{this.Term} ({this.Context.Term})"
			: this.Term;
    }
}
