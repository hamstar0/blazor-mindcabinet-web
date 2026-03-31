using System;
using System.Reflection;


namespace MindCabinet.Shared.Utility;


public static class ShallowEquality {
    public static bool ShallowEquals( this object self, object? other ) {
        if( Object.ReferenceEquals(self, other) ) {
            return true;
        }
        if( other is null ) {
            return false;
        }

        Dictionary<string, object?> leftMembers = ShallowEquality.GetMemberValues(self);
        Dictionary<string, object?> rightMembers = ShallowEquality.GetMemberValues(other);

        if( leftMembers.Count != rightMembers.Count ) {
            return false;
        }

        foreach( KeyValuePair<string, object?> kvp in leftMembers ) {
            if( !rightMembers.TryGetValue(kvp.Key, out var rightValue) ) {
                return false;
            }
            if( !ShallowEquality.MemberValueEquals(kvp.Value, rightValue) ) {
                return false;
            }
        }

        return true;
    }
    
    public static Dictionary<string, object?> GetMemberValues( object instance ) {
        Type type = instance.GetType();
        var values = new Dictionary<string, object?>( StringComparer.Ordinal );

        while( type != null && type != typeof(object) ) {
            PropertyInfo[] props = type.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly
            );
            foreach( var prop in props ) {
                if( prop.GetIndexParameters().Length > 0 ) {
                    continue;
                }
                if( !prop.CanRead ) {
                    continue;
                }

                values[prop.Name] = prop.GetValue( instance );
            }

            FieldInfo[] fields = type.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly
            );
            foreach( var field in fields ) {
                values[field.Name] = field.GetValue( instance );
            }

            type = type.BaseType!;
        }

        return values;
    }

    public static bool MemberValueEquals( object? a, object? b ) {
        if( Object.ReferenceEquals(a, b) ) {
            return true;
        }
        if( a is null || b is null ) {
            return false;
        }

        if( a is string sa && b is string sb ) {
            return sa == sb;
        }

        if( a.GetType().IsValueType || b.GetType().IsValueType ) {
            return a.Equals(b);
        }

        return false; // object references must be the same instance
    }
}
