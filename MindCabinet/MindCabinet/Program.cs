using System;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;
using MindCabinet.Data;
using MindCabinet.Components;
using MindCabinet.Client.Data;
using MindCabinet.Client.Pages;
using Dapper;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MindCabinet;



public class Program {
    public static void Main( string[] args ) {
        var builder = WebApplication.CreateBuilder( args );

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.Configure<ServerDataAccess.ServerDataAccessParameters>( sdaParams => {
            sdaParams.ConnectionString = builder.Configuration.GetConnectionString( "DefaultConnection" )!;
            if( sdaParams.ConnectionString is null ) {
                throw new Exception( "Invalid db connection string." );
            }
        } );

        builder.Services.AddSingleton<SingletonCache>();
        builder.Services.AddSingleton<ClientDataAccess>();
        builder.Services.AddSingleton<ServerDataAccess>();
        builder.Services.AddHttpClient();
        builder.Services.AddControllers();
        
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
