using Dapper;
using Konscious.Security.Cryptography;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Site.Pages;
using MindCabinet.DataObjects;
using MindCabinet.Services;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.DataObjects.Term;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUsers : IServerDataAccess {
    public const string TableName = "SimpleUsers";
    public const string TableColumn_Id = "Id";
    public const string TableColumn_Created = "Created";
    public const string TableColumn_Name = "Name";
    public const string TableColumn_Email = "Email";
    public const string TableColumn_PwHash = "PwHash";
    public const string TableColumn_PwSalt = "PwSalt";
    public const string TableColumn_IsValidated = "IsValidated";

    public async Task<bool> Install_Async(
                IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                {TableColumn_Id} BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                {TableColumn_Created} DATETIME(2) NOT NULL,
                {TableColumn_Name} VARCHAR(128) NOT NULL,
                {TableColumn_Email} VARCHAR(320) NOT NULL,
                {TableColumn_PwHash} BINARY({SimpleUserObject.PasswordHashLength}) NOT NULL,
                {TableColumn_PwSalt} BINARY({SimpleUserObject.PasswordSaltLength}) NOT NULL,
                {TableColumn_IsValidated} BOOLEAN NOT NULL
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );
        
        return true;
    }

    
    public async Task<(bool success, SimpleUserId defaultUserId, TermId defaultUserAsTermId)> Install_After_Async(
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
            parameters: new ClientDataAccess_SimpleUsers.IAPI.Create_Params {
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

        return (true, result.User.Id, result.UserAsTermId!.Id);
    }
}
