using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserContext;
using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MindCabinet.Client.Components.Layout;


public partial class MainLayout : LayoutComponentBase {
    [Inject]
    private ClientSessionData SessionData { get; set; } = null!;
}
