using System;


namespace MindCabinet.Shared.Utility;



public record struct OverridesDefault( bool Value ) {
	public static implicit operator bool( OverridesDefault b ) => b.Value;
	public static implicit operator OverridesDefault( bool b ) => new( b );
}
