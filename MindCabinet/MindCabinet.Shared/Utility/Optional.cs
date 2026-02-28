using System;


namespace MindCabinet.Shared.Utility;



public struct Optional<T> where T : class? {
    public T? Value { get; private set; } = default;

    public bool HasValue { get; private set; } = false;


    public Optional() { }

    public Optional( T? value ) {
        this.Value = value;
        this.HasValue = value is not null;
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


public struct PrimitiveOptional<T> where T : struct {
    public T? Value { get; private set; } = default;


    public PrimitiveOptional( T? value ) {
        this.Value = value;
    }
}
