using MindCabinet.Shared.Utility;

namespace MindCabinet.Client.Services;


public class NetModeClient : INetMode {
    public bool IsClientSide => true;
    public bool IsServerSide => false;
}
