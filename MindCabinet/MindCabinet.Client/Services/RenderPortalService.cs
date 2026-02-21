using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace MindCabinet.Client.Services;


public class RenderPortalService {
    readonly Dictionary<string, Dictionary<Guid, RenderFragment>> Regions = new();

    public event Action? OnChange;



    public Guid Register( string region, RenderFragment fragment ) {
        Dictionary<Guid, RenderFragment> dict;
        if( !this.Regions.TryGetValue(region, out dict!) ) {
            dict = new();
            this.Regions[region] = dict;
        }

        Guid id = Guid.NewGuid();
        dict[id] = fragment;

        this.OnChange?.Invoke();
        return id;
    }

    public void Unregister( string region, Guid id ) {
        if( this.Regions.TryGetValue(region, out var dict) && dict.Remove(id) ) {
            this.OnChange?.Invoke();
        }
    }

    public IEnumerable<RenderFragment> GetFragments( string region ) {
        return this.Regions.TryGetValue(region, out var dict)
            ? dict.Values.ToArray()
            : Array.Empty<RenderFragment>();
    }
}
