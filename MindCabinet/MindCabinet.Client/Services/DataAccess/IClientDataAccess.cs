using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using Microsoft.AspNetCore.SignalR.Client;


namespace MindCabinet.Client.Services.DataAccess;



public partial interface IClientDataAccess : IAsyncDisposable {
}
