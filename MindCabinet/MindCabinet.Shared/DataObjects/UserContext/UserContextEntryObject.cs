using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public partial class UserContextEntryObject( TermObject term, double priority, bool isRequired ) {
    public TermObject Term { get; set; } = term;
    public double Priority { get; set; } = priority;
    public bool IsRequired { get; set; } = isRequired;


    public override string ToString() {
        return $"{this.Term} - {this.Priority} {(this.IsRequired ? "(Required)" : "")}";
    }
}
