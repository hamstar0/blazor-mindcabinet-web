using System.Text.Json.Serialization;

namespace MindCabinet.Shared.DataObjects.Term;


public partial class TermObject {
    public const int MinTermLength = 3;
    public const int MaxTermLength = 64;

    public static ISet<char> AllowedSpecialCharacters = new HashSet<char>(
        ":;,./<>?|[]{}!@#$%^&*-_=+`~".ToCharArray()    // excludes: '\"()\\
    );



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
        
        int consecWhites = 0;
        bool keyboardOnly = term.All( c => {
            if( char.IsLetterOrDigit(c) || TermObject.AllowedSpecialCharacters.Contains(c) ) {
                consecWhites = 0;
                return true;
            }
            if( char.IsWhiteSpace(c) && consecWhites < 2 ) {
                consecWhites++;
                return true;
            }
            return false;
        } );

        return keyboardOnly;
    }
}
