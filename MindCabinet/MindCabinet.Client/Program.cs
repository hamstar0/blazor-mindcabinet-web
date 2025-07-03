using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Runtime.CompilerServices;
using MindCabinet.Client.Services;


namespace MindCabinet.Client;


public class Program {
    public static async Task Main( string[] args ) {
        var builder = WebAssemblyHostBuilder.CreateDefault( args );

        builder.Services.AddScoped( http => new HttpClient {
            BaseAddress = new Uri( builder.HostEnvironment.BaseAddress )
        } );
        builder.Services.AddSingleton<IsClient>();
        builder.Services.AddScoped<ClientDbAccess>();
        builder.Services.AddSingleton<ClientSessionData>();

		//builder.Services.AddHttpClient<IEmployeeService, EmployeeService>( client =>
		//{
		//    client.BaseAddress = new Uri( builder.HostEnvironment.BaseAddress );
		//} );

		///builder.RootComponents.Add<HeadOutlet>("head::after");

		var host = builder.Build();

		//var logger = host.Services.GetRequiredService<ILoggerFactory>()
		//	.CreateLogger<Program>();

		//logger.LogInformation( "Logged after the app is built in the Program file." );

		await host.RunAsync();
	}
}
