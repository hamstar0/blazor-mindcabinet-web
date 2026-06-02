using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MindCabinet.Shared.Utility;


public static class HashHelpers {
    // public static long Combine( params object?[] values ) {
    //     unchecked {
    //         long hash = 17L;
    //         foreach( object? value in values ) {
    //             hash = (hash << 5) - hash + HashHelpers.GetContentHash( value );
    //         }
    //         return hash;
    //     }
    // }

    public static long GetContentHash(
                object? value,
                bool memberContentsAlso = false,
                Dictionary<object, long>? visited = null ) {
        if( value is null ) {
            return 0L;
        }

        if( value is string s ) {
            return StringComparer.Ordinal.GetHashCode(s);
        }

        if( value is ValueType ) {
            return value.GetHashCode();
        }

        visited ??= new Dictionary<object, long>( new ObjectReferenceEqualityComparer() );
        if( visited.ContainsKey(value) ) {
            return visited[value];
        }

        long hash;
        visited[value] = 0L;

        if( value is IEnumerable enumerable ) {
            hash = HashHelpers.GetSequenceHash( enumerable, memberContentsAlso, visited );
        } else if( memberContentsAlso ) {
            hash = HashHelpers.GetReferenceTypeHash( value, visited );
        } else {
            hash = value.GetHashCode();
        }

        visited[value] = hash;

        return hash;
    }

    private static long GetSequenceHash(
                IEnumerable values,
                bool memberContentsAlso = false,
                Dictionary<object, long>? visited = null ) {
        unchecked {
            visited ??= new Dictionary<object, long>( new ObjectReferenceEqualityComparer() );
            if( visited.ContainsKey(values) ) {
                return 0L;
            }

            visited[values] = 0;

            long hash = 17L;
            foreach( object? item in values ) {
                hash = (hash << 5) - hash + HashHelpers.GetContentHash( item, memberContentsAlso, visited );
            }

            visited[values] = hash;
            
            return hash;
        }
    }

    private static long GetReferenceTypeHash(
                object value,
                Dictionary<object, long> visited ) {
        Type type = value.GetType();
        MethodInfo? method = type.GetMethod(
            nameof(GetHashCode),
            BindingFlags.Public | BindingFlags.Instance
        );

        if( method?.DeclaringType != typeof(object) ) {
            return value.GetHashCode();
        }

        return HashHelpers.GetObjectContentHash( type, value, visited );
    }

    private static long GetObjectContentHash( Type type, object value, Dictionary<object, long> visited ) {
        MemberInfo[] members = type.GetProperties( BindingFlags.Public | BindingFlags.Instance )
            .Where( p => p.CanRead && p.GetIndexParameters().Length == 0 )
            .Cast<MemberInfo>()
            .Concat( type.GetFields(BindingFlags.Public | BindingFlags.Instance) )
            .OrderBy( m => m.Name )
            .ToArray();

        long hash = StringComparer.Ordinal.GetHashCode( type.FullName ?? type.Name );

        foreach( MemberInfo member in members ) {
            object? memberValue = member switch {
                PropertyInfo property => property.GetValue(value),
                FieldInfo field => field.GetValue(value),
                _ => null
            };
            hash = (hash << 5) - hash + StringComparer.Ordinal.GetHashCode( member.Name );
            hash = (hash << 5) - hash + HashHelpers.GetContentHash( memberValue, false, visited );
        }

        return hash;
    }
    
    
    
    private sealed class ObjectReferenceEqualityComparer : IEqualityComparer<object> {
        // private bool MemberContentsAlso;
        // public ObjectReferenceEqualityComparer( bool memberContentsAlso ) {
        //     this.MemberContentsAlso = memberContentsAlso;
        // }

        public new bool Equals( object? x, object? y ) => object.ReferenceEquals( x, y );

        public int GetHashCode( object obj ) => RuntimeHelpers.GetHashCode( obj );
    }
}
