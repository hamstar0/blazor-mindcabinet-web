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
                {TableColumn_IsValidated} BOOLEAN NOT NULL,
                 INDEX IDX_{TableColumn_Name} ({TableColumn_Name})
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );
        
        return true;
    }

    
    private ClientDataAccess_SimpleUsers.IAPI.Create_Params DefaultUserParams = new ClientDataAccess_SimpleUsers.IAPI.Create_Params {
        Name = "hamstar",   // temporary!!!!!
        Email = "hamstarhelper@gmail.com",
        Password = "12345A",
        IsValidated = true
    };

    public async Task<(bool success, SimpleUserId defaultUserId)> Install_After_Async(
                IDbConnection dbConnection,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_PostsContexts postsContextDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ServerDataAccess_ServerData serverDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc ) {
        SimpleUserQueryResult result = await this.CreateSimpleUser_Async(
            dbCon: dbConnection,
            termsDataSrc: termsDataSrc,
            postsContextDataSrc: postsContextDataSrc,
            postsContextTermEntryDataSrc: postsContextTermEntryDataSrc,
            serverDataSrc: serverDataSrc,
            userAppDataSrc: userAppDataSrc,
            parameters: this.DefaultUserParams,
            detectCollision: false,
            createUserData: false
        );
        //throw new Exception( JsonSerializer.Serialize(obj) );

        if( result.User is null ) {
            throw new Exception( "Failed to create default user: "+(result.AlreadyExists ? "already exists" : "unknown error") );
        }

        return (true, result.User.Id);
    }
    

    public async Task<(bool success, TermId defaultUserAsTermId)> Install_AfterDefaultUserAndServerData_Async(
                IDbConnection dbConnection,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_PostsContexts postsContextDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ServerDataAccess_ServerData serverDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc,
                SimpleUserId defaultUserId ) {
        (TermObject.Raw defaultUserAsTerm, _) = await this.CreateUserData_Async(
            dbCon: dbConnection,
            termsDataSrc: termsDataSrc,
            postsContextDataSrc: postsContextDataSrc,
            postsContextTermEntryDataSrc: postsContextTermEntryDataSrc,
            serverDataSrc: serverDataSrc,
            userAppDataSrc: userAppDataSrc,
            parameters: this.DefaultUserParams,
            newUserId: defaultUserId
        );

        return (true, defaultUserAsTerm.Id);
    }
}
