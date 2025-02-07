using MindCabinet.Client.Data;
using MindCabinet.Client.Pages;
using MindCabinet.Components;
using MindCabinet.Data;
using System;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.DependencyInjection;


namespace MindCabinet;



public class Program {
    public static void Main( string[] args ) {
        var builder = WebApplication.CreateBuilder( args );

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddSingleton<ClientDataAccess>();  // Obligatory
        builder.Services.AddSingleton<ServerDataAccess>( x => {
            string? connString = builder.Configuration.GetConnectionString( "DefaultConnection" );
            if( connString is null ) {
                throw new Exception( "Invalid db connection string." );
            }
            return ActivatorUtilities.CreateInstance<ServerDataAccess>( x, connString );
        } );
        builder.Services.AddHttpClient();   // Obligatory
        builder.Services.AddControllers();

        //string connectString = @"Data Source = (localdb)\MSSQLLocalDB;
        //    Initial Catalog = ""Mind Cabinet"";
        //    Integrated Security = True;
        //    Connect Timeout = 30;
        //    Encrypt = True;
        //    Trust Server Certificate = False;
        //    Application Intent = ReadWrite;
        //    Multi Subnet Failover = False";
        //builder.Services.AddDbContext<MindCabinetDbContext>( options => options.UseSqlServer( connectionString ) );
        //"ConnectionStrings": {
        //    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-MindCabinet-e18a17f9-0f11-4e92-9c2f-12d63fa72e96;Trusted_Connection=True;MultipleActiveResultSets=true"
        //},
        
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
