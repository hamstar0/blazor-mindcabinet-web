using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;

namespace MindCabinet.Data;



public class ServerSettings {
    public TimeSpan SessionExpirationDuration { get; private set; }
            = new TimeSpan( days: 30, hours: 0, minutes: 0, seconds: 0 );
}
