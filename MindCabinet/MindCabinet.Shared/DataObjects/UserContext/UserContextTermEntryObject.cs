using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextTermEntryObject( TermObject term, double priority, bool isRequired ) {
    public TermObject Term { get; private set; } = term;
    public double Priority { get; private set; } = priority;
    public bool IsRequired { get; private set; } = isRequired;


    public override string ToString() {
        return $"{this.Term} - {this.Priority} {(this.IsRequired ? "(Required)" : "")}";
    }
}
