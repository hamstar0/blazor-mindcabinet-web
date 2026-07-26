using System.Text.RegularExpressions;


namespace MindCabinet.Shared.Utility;


public static class StringHelpers {
    public static string StripWhitespace( this string input ) {
        //Regex.Replace( input, @"\s+", "" )
        return new string(
            input.Where( c => !Char.IsWhiteSpace(c) )
                .ToArray()
        );
    }
    
    
    public static string StripNonAscii( this string input ) {
        return Regex.Replace( input, @"[^\u0020-\u007E]", string.Empty );
    }
    
    
    public static string StripSymbols( this string input ) {
        return new string(
            input.Where( c => Char.IsLetterOrDigit(c) )
                .ToArray()
        );
    }


    public static string ToId( this string input ) {
        return input.StripWhitespace()
            //.StripNonAscii()
            .StripSymbols();
    }
}