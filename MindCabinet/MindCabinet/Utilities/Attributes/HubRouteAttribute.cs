namespace MindCabinet.Utility.Attributes;


[AttributeUsage( AttributeTargets.Class, Inherited = false )]
public class HubRouteAttribute : Attribute {
    public string Route { get; }
    
    
    public HubRouteAttribute( string route ) => this.Route = route;
}
