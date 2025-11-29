using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserContext;


public class UserContextEntry( TermObject term, double priority ) {
    public TermObject Term { get; set; } = term;
    public double Priority { get; set; } = priority;
}
