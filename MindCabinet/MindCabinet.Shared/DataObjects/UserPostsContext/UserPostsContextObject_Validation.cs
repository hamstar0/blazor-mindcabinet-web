using System.Data;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects.Term;

namespace MindCabinet.Shared.DataObjects.UserPostsContext;


public partial class UserPostsContextObject {
    public const int MaxNameLength = 64;



    public static bool ValidateName( string name ) {
        if( !string.IsNullOrWhiteSpace(name) ) {
            return false;
        }
        if( name.Length > MaxNameLength ) {
            return false;
        }
        return true;
    }
}
