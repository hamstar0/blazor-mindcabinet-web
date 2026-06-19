using System.Text.RegularExpressions;


namespace MindCabinet.Shared.Utility;


public static class StringHelpers {
    public static string StripWhitespace( this string input ) {
        return new string(
            input.Where( c => !Char.IsWhiteSpace(c) )
                .ToArray()
        );
    }
    
    
    public static string StripNonAscii( this string input ) {
        return Regex.Replace( input, @"[^\u0020-\u007E]", string.Empty );
    }
}