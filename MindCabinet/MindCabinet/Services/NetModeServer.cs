using MindCabinet.Shared.Utility;

namespace MindCabinet.Services;


public class NetModeServer : INetMode {
    public bool IsClientSide => false;
    public bool IsServerSide => true;
}
