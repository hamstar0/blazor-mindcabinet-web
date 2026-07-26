using MindCabinet.Client.Components.Standard.Tabs;

namespace MindCabinet.Client.Services;


public class TabNavigation {
    private Dictionary<string, TabEntry> Routes = [];
    private Dictionary<string, TabEntry> TabsById = [];



    public void RegisterTabRoute( IEnumerable<string> route, TabEntry tab ) {
        this.Routes[ string.Join(".", route) ] = tab;
        this.TabsById[ tab.Id ] = tab;
    }

    public TabEntry? GetRegisteredTabById( string id ) {
        this.TabsById.TryGetValue( id, out TabEntry? tab );
        return tab;
    }


    public void Navigate( string path ) {
        this.Navigate( path.Split('.') );
    }

    private void Navigate( string[] pathSegs ) {
        foreach( string pathSeg in pathSegs ) {
            TabEntry tab = this.TabsById[pathSeg];
            Tabs tabContainer = tab.TabsContainer;
            
            tabContainer.ChangeTab( pathSeg );
        }
    }
}
