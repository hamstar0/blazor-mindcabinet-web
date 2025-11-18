using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;

namespace MindCabinet.Data;



public partial class ServerSessionData {
    public List<long> FavoriteTermIds { get; private set; } = new List<long>();
}
