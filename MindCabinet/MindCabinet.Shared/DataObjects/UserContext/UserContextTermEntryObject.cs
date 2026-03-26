using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserPostsContext;


public partial class UserPostsContextTermEntryObject( TermObject term, double priority, bool isRequired ) {
    //public UserPostsContextObject UserPostsContext { get; } = userPostsContext;
    public TermObject Term { get; } = term;

    public double Priority { get; } = priority;

    public bool IsRequired { get; } = isRequired;

    
    public override string ToString() {
        return $"{this.Term} - {this.Priority} {(this.IsRequired ? "(Required)" : "")}";
    }
}
