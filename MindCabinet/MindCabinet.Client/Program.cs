using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Runtime.CompilerServices;
using MindCabinet.Client.Services;
using System.Reflection;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Client.Services.DataPresenters;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Client;


public class Program {
    private static IEnumerable<Type> GetInterfaceImplementations( Type interfaceType ) {
        IEnumerable<Type> implementations = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany( a => a.GetTypes() )
            .Where( t => interfaceType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract );

        return implementations;
    }



    public static async Task Main( string[] args ) {
        var builder = WebAssemblyHostBuilder.CreateDefault( args );

        builder.Services.AddSingleton<INetMode, NetModeClient>();

        builder.Services.AddScoped( http => new HttpClient {
            BaseAddress = new Uri( builder.HostEnvironment.BaseAddress )
        } );

        IEnumerable<Type> dataAccessServiceTypes = Program.GetInterfaceImplementations( typeof(IClientDataAccess) );
        foreach( Type implementation in dataAccessServiceTypes ) {
            builder.Services.AddScoped( implementation );
        }
        IEnumerable<Type> dataProcessorServiceTypes = Program.GetInterfaceImplementations( typeof(IClientDataProcessors) );
        foreach( Type implementation in dataProcessorServiceTypes ) {
            builder.Services.AddScoped( implementation );
        }

        builder.Services.AddSingleton<ClientSessionData>();

		var host = builder.Build();

// System.Diagnostics.Debug.WriteLine($"2 ADDRESS ISSUE {baseAddress1} vs {builder.HostEnvironment.BaseAddress} vs {Program.BaseAddress}");
// Console.WriteLine($"2 ADDRESS ISSUE {baseAddress1} vs {builder.HostEnvironment.BaseAddress} vs {Program.BaseAddress}");

		//var logger = host.Services.GetRequiredService<ILoggerFactory>()
		//	.CreateLogger<Program>();
		//logger.LogInformation( "Logged after the app is built in the Program file." );

		await host.RunAsync();
	}
}


