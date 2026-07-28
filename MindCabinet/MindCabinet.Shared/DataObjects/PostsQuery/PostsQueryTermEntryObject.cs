using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsQuery;


public partial class PostsQueryTermEntryObject( TermObject term, double priority, bool isRequired ) {
    //public PostsQueryObject PostsQuery { get; } = postsQuery;
    public TermObject Term { get; private set; } = term;

    public double Priority { get; private set; } = priority;

    public bool IsRequired { get; private set; } = isRequired;

    

    public PostsQueryTermEntryObject Clone() {
        return new PostsQueryTermEntryObject(
            term: this.Term,
            priority: this.Priority,
            isRequired: this.IsRequired
        );
    }


    public bool IsValid() {
        if( this.Term == null || this.Term.Id == default ) {
            return false;
        }
        return true;
    }


    public void SetTerm( TermObject newTerm ) {
        this.Term = newTerm;
    }

    public void SetPriority( double newPriority ) {
        this.Priority = newPriority;
    }
    
    public void SetIsRequired( bool newIsRequired ) {
        this.IsRequired = newIsRequired;
    }


    public override string ToString() {
        return $"{this.Term} - {this.Priority} {(this.IsRequired ? "(Required)" : "")}";
    }
}
