using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using MindCabinet.Utility.Attributes;


namespace MindCabinet.Extensions;



public static class SignalRHubExtensions {
    public static void MapHubs( this IEndpointRouteBuilder endpoints ) {
        // Find all classes in the current assembly inheriting from Hub
        IEnumerable<Type> hubTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where( t => t.IsClass && !t.IsAbstract && typeof(Hub).IsAssignableFrom(t) );
        if( hubTypes.Count() == 0 ) {
            throw new Exception( "No Hub classes found in the assembly!" );
        }

        MethodInfo mapHubMethod = typeof(HubEndpointRouteBuilderExtensions)
            .GetMethod(
                name: nameof( HubEndpointRouteBuilderExtensions.MapHub ), 
                types: new[] { typeof(IEndpointRouteBuilder), typeof(string) }
            )!;
        if( mapHubMethod is null ) {
            throw new Exception( "Unable to find MapHub method via reflection!" );
        }

        foreach( Type? hubType in hubTypes ) {
            // Extract our custom route attribute
            var attribute = hubType.GetCustomAttribute<HubRouteAttribute>();
            if( attribute is null ) {
                continue;
            }

            // This dynamically calls app.MapHub<THub>(route) for each hub type with the HubRouteAttribute
            mapHubMethod
                .MakeGenericMethod( hubType )
                .Invoke( null, new object[] { endpoints, "/"+attribute.Route } );
        }
    }
}
