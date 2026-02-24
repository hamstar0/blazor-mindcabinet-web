namespace MindCabinet.Shared.DataObjects;


public class IdDataObject<T> {
    public long Id { get; set; }

    public T? Data { get; set; }
}