using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Runtime.CompilerServices;
using MindCabinet.Client.Services;
using System.Reflection;
using MindCabinet.Client.Services.DataAccess;


namespace MindCabinet.Client;


public class Program {
	private static IEnumerable<Type> GetAllTypesImplementingInterface<IType>() {
        var mytype = typeof(IType);
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(s => s.GetTypes())
			.Where(p => mytype.IsAssignableFrom(p));
    }

	private static void CallMethodWithGenericParameter( IServiceCollection services, Type genericParamType ) {
        MethodInfo? method = services.GetType()
			.GetMethod( "AddScoped" );
		MethodInfo? genericMethod = method?.MakeGenericMethod( genericParamType );

		genericMethod?.Invoke( services, null );

		if( genericMethod is null ) {
			throw new NullReferenceException( "Could not create generic method." );
		}
    }

	private static void RegisterAllClassesOfInterface<IType>( IServiceCollection services ) {
		IEnumerable<Type> dbAccessTypes = Program.GetAllTypesImplementingInterface<IType>();

		foreach( Type dbAccessType in dbAccessTypes ) {
			Program.CallMethodWithGenericParameter( services, dbAccessType );
		}
	}



    public static async Task Main( string[] args ) {
        var builder = WebAssemblyHostBuilder.CreateDefault( args );

        builder.Services.AddScoped( http => new HttpClient {
            BaseAddress = new Uri( builder.HostEnvironment.BaseAddress )
        } );
        builder.Services.AddSingleton<IsClient>();

		Program.RegisterAllClassesOfInterface<IClientDataAccess>( builder.Services );

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
