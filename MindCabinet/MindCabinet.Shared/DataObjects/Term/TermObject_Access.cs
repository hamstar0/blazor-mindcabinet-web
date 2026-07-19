using System.Text.Json.Serialization;
using MindCabinet.Shared.Utility;
using MindCabinet.Shared.DataObjects;

namespace MindCabinet.Shared.DataObjects.Term;



public partial class TermObject : IEquatable<TermObject>, IComparable, IComparable<TermObject>, IDataObject {
    public bool SetTerm( string term ) {
        if( !TermObject.ValidateTerm(term) ) {
            return false;
        }
        this.Term = term;
        return true;
    }

    public bool SetAbbreviation( string abbrev ) {
        if( !TermObject.ValidateTerm(abbrev) ) {
            return false;
        }
        this.Abbreviation = abbrev;
        return true;
    }

    public void SetDescription( string desc ) {
        this.Description = desc;
    }

    public void SetContext( TermObject.Raw context ) {
        this.Context = context;
    }

    public void SetAlias( TermObject.Raw alias ) {
        this.Alias = alias;
    }
}
