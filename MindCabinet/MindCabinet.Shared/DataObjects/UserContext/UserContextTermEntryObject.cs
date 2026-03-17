using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextTermEntryObject( TermObject term, double priority, bool isRequired ) {
    //public UserContextObject UserContext { get; } = userContext;
    public TermObject Term { get; } = term;

    public double Priority { get; } = priority;

    public bool IsRequired { get; } = isRequired;


    public UserContextTermEntryObject.Raw ToRaw() {
        return new UserContextTermEntryObject.Raw {
            TermId = this.Term.Id,
            Priority = this.Priority,
            IsRequired = this.IsRequired
        };
    }
    
    public override string ToString() {
        return $"{this.Term} - {this.Priority} {(this.IsRequired ? "(Required)" : "")}";
    }
}
