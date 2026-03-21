using System.Text.Json.Serialization;
using MindCabinet.Shared.Utility;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject>, IDataObject {
    public long Id { get; }

    public string Term { get; }

    public TermObject.Raw? Context { get; }

    public TermObject.Raw? Alias { get; }



	public TermObject( long id, string term, TermObject? context, TermObject? alias ) {
		if( id == 0 ) {
			throw new ArgumentException( "Id cannot be 0 in TermObject." );
		}

		this.Id = id;
		this.Term = term;
		this.Context = context is not null
            ? new TermObject.Raw {
                Id = context.Id,
                Term = context.Term,
                ContextTermId = context.Context?.Id,
                AliasTermId = context.Alias?.Id
            }
            : null;
		this.Alias = alias is not null
            ? new TermObject.Raw {
                Id = alias.Id,
                Term = alias.Term,
                ContextTermId = alias.Context?.Id,
                AliasTermId = alias.Alias?.Id
            }
            : null;
    }

	public TermObject( long id, string term, TermObject.Raw? context, TermObject.Raw? alias ) {
		this.Id = id;
		this.Term = term;
		this.Context = context;
		this.Alias = alias;
    }


    public override int GetHashCode() {
        return HashCode.Combine(
			this.Id,
			this.Term,
            this.Context is null
				? 0 : HashCode.Combine( this.Context.Id, this.Context?.Term ),
            this.Alias is null
				? 0 : HashCode.Combine( this.Alias.Id, this.Alias?.Term )
		);
    }

    public override bool Equals( object? obj ) {
        return obj is TermObject
			? this.EqualsTermShallow( obj as TermObject )
            : base.Equals( obj );
    }

	public bool Equals( TermObject? other ) {
		return this.EqualsTermShallow( other );
	}

    public bool EqualsTermShallow( TermObject? other ) {
		if( other is null ) { return false; }
		if( ReferenceEquals(this, other) ) { return true; }

        if( this.Id != other.Id ) {
            return false;
        }

		if( !this.Term.Equals(other.Term) ) {
            return false;
        }
		if( this.Context?.Id != other.Context?.Id ) {
            return false;
        }
		// if( this.Alias?.Id != other.Alias?.Id ) {
        //     return false;
        // }

		return true;
    }

    public int CompareTo( object? obj ) {
        if( obj is not TermObject ) {
            return 1;
        }
        return this.CompareTo( obj as TermObject );
    }

    public int CompareTo( TermObject? test ) {
        if( test is null ) {
            return 1;
        }
        return this.ToString()
            .CompareTo( test?.ToString() );   // todo?
        //return this.SafeCompareTo( test, null );
    }


    public override string ToString() {
		return this.Context is not null
            ? $"{this.Term} ({this.Context.Term})"
			: this.Term;
    }

    // public Prototype ToPrototype() {
    //     return new Prototype {
    //         Id = this.Id,
    //         Term = this.Term,
    //         ContextTermId = new PrimitiveOptional<long>( value: this.Context?.Id ),
    //         AliasTermId = new PrimitiveOptional<long>( value: this.Alias?.Id )
    //     };
    // }
}
