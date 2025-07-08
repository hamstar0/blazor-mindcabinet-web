using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Shared.DataEntries;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MindCabinet.Client.Layout;


public partial class MainLayout : LayoutComponentBase {
    //[Inject]
    //public IJSRuntime Js { get; set; } = null!;

    //[Inject]
    //public HttpClient Http { get; set; } = null!;

    //[Inject]
    //public ClientDbAccess DbAccess { get; set; } = null!;

    [Inject]
    public ClientSessionData SessionData { get; set; } = null!;
}
