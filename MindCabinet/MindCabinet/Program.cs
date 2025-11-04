using System.Data;
using MindCabinet.Client.Pages;
using MindCabinet.Client.Services;
using MindCabinet.Components;
using MindCabinet.Data;


namespace MindCabinet;



public class Program {
    public static void Main( string[] args ) {
        var builder = WebApplication.CreateBuilder( args );

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        string? conn = builder.Configuration.GetConnectionString( "DefaultConnection" );
        if( string.IsNullOrEmpty(conn) ) {
            throw new Exception( "No connection string configured!" );
        }

        //builder.Services.AddDistributedMemoryCache();
        //builder.Services.AddSession( options => {
        //    options.IdleTimeout = TimeSpan.FromHours( 24 );  // expire time
        //    options.Cookie.Expiration = TimeSpan.FromHours( 24 );
        //    options.Cookie.HttpOnly = true;
        //    options.Cookie.IsEssential = true;
        //} );
        // builder.Services.AddControllersWithViews();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSingleton<ServerSettings>();
        builder.Services.AddScoped<ClientDbAccess>();  // not AddSingleton?

        builder.Services.AddTransient<Func<IDbConnection>>( sp => () => new MySqlConnector.MySqlConnection( conn ) );
        builder.Services.AddTransient<ServerDbAccess>();  // not AddSingleton

        builder.Services.AddScoped<ServerSessionData>();
        builder.Services.AddTransient<ClientSessionData>(); // Unused, but needed for components
        builder.Services.AddHttpClient();
        builder.Services.AddControllers();

        //
        
        WebApplication app = builder.Build();

        app.UseStatusCodePagesWithReExecute( "/StatusCode/{0}" );

        // Configure the HTTP request pipeline.
        if ( app.Environment.IsDevelopment() ) {
            app.UseWebAssemblyDebugging();
        } else {
            app.UseExceptionHandler( "/Error" );
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        //app.UseSession();

        app.MapControllers();
        //app.MapFallbackToPage("");
        //app.MapFallbackToFile("404page.html");

        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies( typeof(Home).Assembly );


        //var logger = app.Services.GetRequiredService<ILogger<Program>>();
        //app.Lifetime.ApplicationStarted.Register( () => {
        //    logger.LogInformation("=== Starting Endpoint Inspection ===");

        // // Log all mapped routes after UseRouting and before UseEndpoints
        // app.Use( async ( context, next ) => {
        //     var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        //     var endpointSources = context.RequestServices.GetRequiredService<IEnumerable<EndpointDataSource>>();

        //     foreach( var endpointSource in endpointSources ) {
        //         foreach( var endpoint in endpointSource.Endpoints ) {
        //             if( endpoint is RouteEndpoint routeEndpoint ) {
        //                 logger.LogInformation( "Mapped Route: {RoutePattern}", routeEndpoint.RoutePattern.RawText );
        //                 // You can log more details if needed, e.g., routeEndpoint.Metadata, routeEndpoint.DisplayName
        //             }
        //         }
        //     }

        //     await next();
        // } );
        

        app.Run();
    }
}
