namespace MindCabinet.Shared.Utility;

public interface INetMode {
    bool IsClientSide { get; }
    bool IsServerSide { get; }
}