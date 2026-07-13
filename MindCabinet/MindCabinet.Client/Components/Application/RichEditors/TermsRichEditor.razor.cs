using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Components.Application;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DataPresenters;
using System.Text;

namespace MindCabinet.Client.Components.Application.RichEditors;


public partial class TermsRichEditor : ComponentBase {
    [Parameter]
    public string? AddedClasses { get; set; } = null;
}
