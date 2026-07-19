using System.Text.Json.Serialization;
using MindCabinet.Shared.Utility;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.Term;


public enum TermId : long { }



public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject>, IDataObject {   //IHasId<TermId>
    public TermId Id { get; }

    public string Term { get; private set; }

    public string? Abbreviation { get; private set; }

    public string? Description { get; private set; }

    public TermObject.Raw? Context { get; private set; }

    public TermObject.Raw? Alias { get; private set; }



	public TermObject( TermObject copy ) : this(
        id: copy.Id,
        term: copy.Term,
        abbreviation: copy.Abbreviation,
        description: copy.Description,
        context: copy.Context,
        alias: copy.Alias
    ) { }

	public TermObject(
                TermId id,
                string term,
                string? abbreviation,
                string? description,
                TermObject? context,
                TermObject? alias ) {
		if( id == 0 ) {
			throw new ArgumentException( "Id cannot be 0 in TermObject." );
		}

		this.Id = id;
		this.Term = term;
		this.Abbreviation = abbreviation;
		this.Description = description;
		this.Context = context is not null
            ? TermObject.CreateRaw(
                id: context.Id,
                term: context.Term,
                abbreviation: this.Abbreviation,
                description: this.Description,
                contextId: context.Context?.Id,
                aliasId: context.Alias?.Id
            )
            : null;
		this.Alias = alias is not null
            ? TermObject.CreateRaw(
                id: alias.Id,
                term: alias.Term,
                abbreviation: this.Abbreviation,
                description: this.Description,
                contextId: alias.Context?.Id,
                aliasId: alias.Alias?.Id
            )
            : null;
    }

	public TermObject(
                TermId id,
                string term, 
                string? abbreviation,
                string? description,
                TermObject.Raw? context,
                TermObject.Raw? alias ) {
		this.Id = id;
		this.Term = term;
		this.Abbreviation = abbreviation;
		this.Description = description;
		this.Context = context;
		this.Alias = alias;
    }


    public override int GetHashCode() {
        return HashCode.Combine(
			this.Id,
			this.Term,
            this.Abbreviation,
            this.Description,
            this.Context is null
				? 0 : HashCode.Combine( this.Context.Id, this.Context?.Term ),
            this.Alias is null
				? 0 : HashCode.Combine( this.Alias.Id, this.Alias?.Term )
		);
    }

    public override bool Equals( object? obj ) {
        return obj is TermObject
			? this.EqualsTermShallow( obj as TermObject )
            : base.Equals( obj );   // Equatable anonymous object?
    }

	public bool Equals( TermObject? other ) {
		return this.EqualsTermShallow( other );
	}

    public bool EqualsTermShallow( TermObject? other ) {
		if( other is null ) { return false; }
		if( Object.ReferenceEquals(this, other) ) { return true; }

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


    public string ToId( bool verbose ) {
        if( verbose ) {
            return (this.Abbreviation is not null ? this.Abbreviation : this.Term)
                .StripWhitespace()
                .StripNonAscii()+"_"+this.Id;
        } else {
            return this.Id.ToString();
        }
    }

    public override string ToString() {
        string term = this.Abbreviation is not null ? this.Abbreviation : this.Term;
        
		return this.Context is not null
            ? $"{term} ({this.Context.Term})"
			: term;
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
