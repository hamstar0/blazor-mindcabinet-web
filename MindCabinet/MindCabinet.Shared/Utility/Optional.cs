using System;


namespace MindCabinet.Shared.Utility;



public struct Optional<T> where T : class? {
    public T? Value { get; private set; } = default;

    public bool HasValue { get; private set; } = false;


    public Optional() { }

    public Optional( T value ) {
        this.Value = value;
        this.HasValue = true;
    }

    public void Set( T value ) {
        this.Value = value;
        this.HasValue = true;
    }

    public void Unset() {
        this.Value = default;
        this.HasValue = false;
    }
}
