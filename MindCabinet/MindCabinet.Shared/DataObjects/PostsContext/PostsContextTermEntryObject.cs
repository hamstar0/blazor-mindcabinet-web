using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.PostsContext;


public partial class PostsContextTermEntryObject( TermObject term, double priority, bool isRequired ) {
    //public PostsContextObject PostsContext { get; } = postsContext;
    public TermObject Term { get; } = term;

    public double Priority { get; } = priority;

    public bool IsRequired { get; } = isRequired;

    
    public override string ToString() {
        return $"{this.Term} - {this.Priority} {(this.IsRequired ? "(Required)" : "")}";
    }
}
