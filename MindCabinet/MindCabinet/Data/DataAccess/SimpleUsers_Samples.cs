using Dapper;
using Konscious.Security.Cryptography;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Site.Pages;
using MindCabinet.DataObjects;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUsers : IServerDataAccess {
    public async Task<(bool success, SimpleUserId defaultUserId)> Install_After_Async(
                IDbConnection dbConnection,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_PostsContexts postsContextData,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryData,
                ServerDataAccess_ServerData serverData,
                ServerDataAccess_UserAppData userAppData ) {
        SimpleUserQueryResult result = await this.CreateSimpleUser_Async(
            dbCon: dbConnection,
            termsData: termsData,
            postsContextData: postsContextData,
            postsContextTermEntryData: postsContextTermEntryData,
            serverData: serverData,
            userAppData: userAppData,
            parameters: new ClientDataAccess_SimpleUsers.Create_Params {
                Name = "hamstar",   // temporary!!!!!
                Email = "hamstarhelper@gmail.com",
                Password = "12345A",
                IsValidated = true
            },
            detectCollision: false,
            createPostsContext: true
        );
        //throw new Exception( JsonSerializer.Serialize(obj) );

        if( result.User is null ) {
            throw new Exception( "Failed to create default user: "+(result.AlreadyExists ? "already exists" : "unknown error") );
        }

        return (true, result.User.Id);
    }
}
