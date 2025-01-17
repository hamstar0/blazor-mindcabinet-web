using MindCabinet.Client.Data;
using MindCabinet.Client.Pages;
using MindCabinet.Components;
using MindCabinet.Data;


namespace MindCabinet;



public class Program {
    public static void Main( string[] args ) {
        var builder = WebApplication.CreateBuilder( args );

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddSingleton<ClientDataAccess>();  // Obligatory
        builder.Services.AddSingleton<ServerDataAccess>();
        builder.Services.AddHttpClient();   // Obligatory
        builder.Services.AddControllers();

		//"ConnectionStrings": {
		//    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-MindCabinet-e18a17f9-0f11-4e92-9c2f-12d63fa72e96;Trusted_Connection=True;MultipleActiveResultSets=true"
		//},
		//var connectionString = builder.Configuration.GetConnectionString( "DefaultConnection" );
		//builder.Services.AddDbContext<MindCabinetDbContext>( options => options.UseSqlServer(connectionString) );

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

        app.MapControllers();
        //app.MapFallbackToPage("");
        //app.MapFallbackToFile("404page.html");

        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies( typeof(Home).Assembly );

        app.Run();
    }
}
