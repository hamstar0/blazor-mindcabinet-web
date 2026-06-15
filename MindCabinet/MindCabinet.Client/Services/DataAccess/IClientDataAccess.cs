using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Threading;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.Client.Services.DataAccess;



public partial interface IClientDataAccess {    //: IAsyncDisposable {
    public static async Task<TOutput> CallAPI_Async<TInput, TOutput>(     // do not use `string` for `parameters`
                HttpClient http,
                string route,
                TInput parameters ) {
        HttpResponseMessage msg = await http.PostAsJsonAsync(
            requestUri: route,
            value: parameters
        );

        msg.EnsureSuccessStatusCode();

        TOutput? ret = await msg.Content.ReadFromJsonAsync<TOutput>();
        if( ret is null ) {
            throw new InvalidDataException( $"Could not deserialize {typeof(TOutput).FullName}" );
        }
        
        return ret;
    }

    public static async Task<TOutput> CallAPI_Async<TOutput>(
                HttpClient http,
                string route ) {
        return await CallAPI_Async<object, TOutput>( http, route, new object() );
    }

    public static async Task CallAPI_Async<TInput>(     // do not use `string` for `parameters`
                HttpClient http,
                string route,
                TInput parameters ) {
        await CallAPI_Async<TInput, object>( http, route, parameters );
    }


    public static async Task<TOutput> CallAPIManual_Async<TInput, TOutput>(     // do not use `string` for `parameters`
                HttpClient http,
                string route,
                TInput parameters ) {
        JsonContent content = JsonContent.Create( parameters, mediaType: null, null );

        HttpResponseMessage msg = await http.PostAsync(
            requestUri: route,
            content: content,
            cancellationToken: default
        );

        msg.EnsureSuccessStatusCode();

        TOutput? ret = await msg.Content.ReadFromJsonAsync<TOutput>();
        if( ret is null ) {
            throw new InvalidDataException( $"Could not deserialize {typeof(TOutput).FullName}" );
        }
        
        return ret;
    }
}
