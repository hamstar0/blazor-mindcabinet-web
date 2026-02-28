using System.Text.Json.Serialization;
using MindCabinet.Shared.Utility;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject> {
    public long Id { get; private set; }

    public string Term { get; set; }

    public IdDataObject<TermObject>? Context { get; private set; }

    public IdDataObject<TermObject>? Alias { get; private set; }



	public TermObject( long id, string term, TermObject? context, TermObject? alias ) {
		this.Id = id;
		this.Term = term;
		this.Context = context is not null ? new IdDataObject<TermObject> { Id = context.Id, Data = context } : null;
		this.Alias = alias is not null ? new IdDataObject<TermObject> { Id = alias.Id, Data = alias } : null;
    }

	public TermObject( long id, string term, long? contextId, long? aliasId ) {
		this.Id = id;
		this.Term = term;
		this.Context = contextId is not null ? new IdDataObject<TermObject> { Id = contextId.Value, Data = null } : null;
		this.Alias = aliasId is not null ? new IdDataObject<TermObject> { Id = aliasId.Value, Data = null } : null;
    }


    public override int GetHashCode() {
        return HashCode.Combine(
			this.Id,
			this.Term,
            this.Context is null
				? 0 : HashCode.Combine( this.Context.Id, this.Context.Data?.Term ),
            this.Alias is null
				? 0 : HashCode.Combine( this.Alias.Id, this.Alias.Data?.Term )
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
			? this.Context.Data is not null
                ? $"{this.Term} ({this.Context.Data.Term})"
                : this.Term+" {"+this.Context.Id+"}"
			: this.Term;
    }

    public Prototype ToPrototype() {
        // IdDataObject<TermObject>? ctx = this.Context is not null 
        //     ? new IdDataObject<TermObject> { Id = this.Context.Id, Data = this.Context.Data }
        //     : null;
        // IdDataObject<TermObject>? alias = this.Alias is not null 
        //     ? new IdDataObject<TermObject> { Id = this.Alias.Id, Data = this.Alias.Data }
        //     : null;

        return new Prototype {
            Id = this.Id,
            Term = this.Term,
            ContextTermId = new PrimitiveOptional<long>( value: this.Context?.Id ),
            AliasTermId = new PrimitiveOptional<long>( value: this.Alias?.Id )
        };
    }
}
