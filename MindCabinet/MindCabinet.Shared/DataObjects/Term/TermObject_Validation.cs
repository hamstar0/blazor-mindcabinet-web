using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject {
    public const int MinTermLength = 3;
    public const int MaxTermLength = 64;



    public static bool ValidateTerm( string term ) {
        if( string.IsNullOrWhiteSpace(term) ) {
            return false;
        }
        if( term.Length < MinTermLength ) {
            return false;
        }
        if( term.Length > MaxTermLength ) {
            return false;
        }
        return true;
    }
}
