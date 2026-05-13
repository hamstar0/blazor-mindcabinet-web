using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsContext;


public partial class PostsContextTermEntryObject( TermObject term, double priority, bool isRequired ) {
    //public PostsContextObject PostsContext { get; } = postsContext;
    public TermObject Term { get; private set; } = term;

    public double Priority { get; private set; } = priority;

    public bool IsRequired { get; private set; } = isRequired;

    

    public PostsContextTermEntryObject Clone() {
        return new PostsContextTermEntryObject(
            term: this.Term,
            priority: this.Priority,
            isRequired: this.IsRequired
        );
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
