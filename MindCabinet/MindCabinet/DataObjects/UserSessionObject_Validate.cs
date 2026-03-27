using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;
using System.Net;


namespace MindCabinet.DataObjects;


public partial class UserSessionObject {
    public const int MaxNameLength = 64;



    public static bool ValidateId( string id ) {
        if( string.IsNullOrWhiteSpace(id) ) {
            return false;
        }
        if( !Guid.TryParse(id, out Guid _) ) {
            return false;
        }

        return true;
    }

    public static bool ValidateIpAddress( string ipAddress ) {
        if( string.IsNullOrWhiteSpace(ipAddress) ) {
            return false;
        }
        if( !IPAddress.TryParse(ipAddress, out IPAddress? _) ) {
            return false;
        }

        return true;
    }
}
