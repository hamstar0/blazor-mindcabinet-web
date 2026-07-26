using MindCabinet.Shared.Utility;

namespace MindCabinet.Client.Services;


public class NetModeClient : INetMode {
    public bool IsClientSide => true;   // stfu
    public bool IsServerSide => false;
}
