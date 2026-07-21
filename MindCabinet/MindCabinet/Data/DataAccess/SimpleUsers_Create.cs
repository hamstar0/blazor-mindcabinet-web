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
using MindCabinet.Shared.Utility;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimpleUsers : IServerDataAccess {
    public async Task<SimpleUserQueryResult> CreateSimpleUser_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_PostsContexts postsContextDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ServerDataAccess_ServerData serverDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc,
                ClientDataAccess_SimpleUsers.IAPI.Create_Params parameters,
                bool detectCollision,
                bool createUserData ) {
        SimpleUserObject.StatusCode code;
        code = SimpleUserObject.GetUserNameStatus(parameters.Name);
        if( code != SimpleUserObject.StatusCode.OK ) {
            throw new ArgumentException( $"User name is not valid ({code.ToString()})." );
        }
        code = SimpleUserObject.GetEmailStatus(parameters.Email);
        if( code != SimpleUserObject.StatusCode.OK ) {
            throw new ArgumentException( $"Email is not valid ({code.ToString()})." );
        }
        code = SimpleUserObject.GetPasswordStatus(parameters.Password);
        if( code != SimpleUserObject.StatusCode.OK ) {
            throw new ArgumentException( $"Password is not valid ({code.ToString()})." );
        }

        SimpleUserObject.Raw? user;

        if( detectCollision ) {
            user = await dbCon.QuerySingleOrDefaultAsync<SimpleUserObject.Raw?>(
                $"SELECT * FROM {TableName} WHERE {TableColumn_Name} = @Name",
                new { Name = parameters.Name }
            );
            if( user is not null ) {
                return new SimpleUserQueryResult( null, null, null, true );
            }
        }

        DateTime now = DateTime.UtcNow;

        byte[] pwSalt = ServerDataAccess_SimpleUsers.GeneratePasswordSalt();
        byte[] pwHash = ServerDataAccess_SimpleUsers.GeneratePasswordHash( parameters.Password, pwSalt );

        long newUserId = await dbCon.ExecuteScalarAsync<long>(
            $@"INSERT INTO {TableName} (
                    {TableColumn_Created},
                    {TableColumn_Name},
                    {TableColumn_Email},
                    {TableColumn_PwHash},
                    {TableColumn_PwSalt},
                    {TableColumn_IsValidated}
                ) 
                VALUES (@Created, @Name, @Email, @PwHash, @PwSalt, @IsValidated);
            SELECT LAST_INSERT_ID();",  //OUTPUT INSERTED.Id 
            new {
                Created = now,
                Name = parameters.Name,
                Email = parameters.Email,
                PwHash = pwHash,
                PwSalt = pwSalt,
                IsValidated = parameters.IsValidated
            }
        );

        var newUser = SimpleUserObject.CreateRaw(
            id: (SimpleUserId)newUserId,
            created: now,
            name: parameters.Name,
            email: parameters.Email,
            pwHash: pwHash,
            pwSalt: pwSalt,
            isValidated: parameters.IsValidated    // note: not for client
            //isPrivileged: isPrivileged
        );

        //

        var userRaw = SimpleUserObject.CreateRaw(
            id: newUser.Id,
            created: now,
            name: parameters.Name,
            email: parameters.Email,
            pwHash: pwHash,
            pwSalt: pwSalt,
            isValidated: parameters.IsValidated
        );

        ServerDataAccess_SimpleUsers.Cache_ById.Set(
            key: userRaw.Id,
            value: userRaw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );
        ServerDataAccess_SimpleUsers.Cache_ByName.Set(
            key: userRaw.Name,
            value: userRaw.Id,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        TermObject.Raw? userTerm = null;
        PostsContextObject.Raw? userDefaultPostsContext = null;

        if( createUserData ) {
            (userTerm, userDefaultPostsContext) = await this.CreateUserData_Async(
                dbCon: dbCon,
                serverDataSrc: serverDataSrc,
                userAppDataSrc: userAppDataSrc,
                termsDataSrc: termsDataSrc,
                postsContextDataSrc: postsContextDataSrc,
                postsContextTermEntryDataSrc: postsContextTermEntryDataSrc,
                parameters: parameters,
                newUserId: newUser.Id
            );
        }

        //

        return new SimpleUserQueryResult( newUser, userDefaultPostsContext, userTerm, false );
    }

    private async Task<TermObject.Raw> CreateUserTerm_Async(
                IDbConnection dbCon,
                ServerDataAccess_ServerData serverDataSrc,
                ServerDataAccess_Terms termsDataSrc,
                string userName,
                SimpleUserId creator ) {
        ServerDataObject.Raw? serverDataObj = await serverDataSrc.Get_Async( dbCon );
        if( serverDataObj is null ) {
            throw new Exception( "Server application data not found." );
        }
        if( serverDataObj?.UsersConceptTermId is null || serverDataObj?.UsersConceptTermId == 0 ) {
            throw new Exception( "User Concept term not found." );
        }

        return ( await termsDataSrc.Create_Async(
            dbCon,
            creator,
            new ClientDataAccess_Terms.IAPI.CreateForCurrentUser_Params {
                TermBody = userName,
                ContextId = serverDataObj?.UsersConceptTermId
            }
        ) ).TermRaw;
    }

    
    private async Task<PostsContextObject.Raw> CreateDefaultUserPostsContext_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContexts postsContextDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ClientDataAccess_SimpleUsers.IAPI.Create_Params parameters,
                // TermId userAsTermId,
                SimpleUserId ownerUserId ) {
        PostsContextObject.Prototype proto = new PostsContextObject.Prototype {
            Name = $"{parameters.Name}'s posts",
            Description = "All posts by the given user.",
            Owner = ownerUserId,
            Entries = [
                // new PostsContextTermEntryObject.Prototype {
                //     TermId = userAsTermId,
                //     Priority = 1,
                //     IsRequired = true
                // }
            ]
        };
        PostsContextId defaultCtxId = (await postsContextDataSrc.Create_Async(
            dbCon: dbCon,
            postsContextTermEntryDataSrc: postsContextTermEntryDataSrc,
            parameters: proto,
            owner: ownerUserId
        )).Id;

        proto.Id = defaultCtxId;
        //proto.Entries[0].PostsContextId = defaultCtxId;

        // PostsContextObject.Raw rawCtx = PostsContextObject.CreateRaw(
        //     id: defaultCtxId,
        //     name: proto.Name,
        //     description: proto.Description,
        //     entries: proto.Entries
        // );
        // PostsContextObject ctx = await ServerDataAccess_PostsContexts
        //     .ToDataObject_Async( dbCon, termsData, rawCtx );

        return proto.ToRaw(true);
    }
    
    internal async Task<(TermObject.Raw, PostsContextObject.Raw)> CreateUserData_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_PostsContexts postsContextDataSrc,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ServerDataAccess_ServerData serverDataSrc,
                ServerDataAccess_UserAppData userAppDataSrc,
                ClientDataAccess_SimpleUsers.IAPI.Create_Params parameters,
                SimpleUserId newUserId ) {
        TermObject.Raw userTerm = await this.CreateUserTerm_Async(
            dbCon: dbCon,
            serverDataSrc: serverDataSrc,
            termsDataSrc: termsDataSrc,
            userName: parameters.Name,
            creator: newUserId
        );

        PostsContextObject.Raw userDefaultPostsContext = await this.CreateDefaultUserPostsContext_Async(
            dbCon: dbCon,
            postsContextDataSrc: postsContextDataSrc,
            postsContextTermEntryDataSrc: postsContextTermEntryDataSrc,
            parameters: parameters,
            ownerUserId: newUserId
        );
        
        await userAppDataSrc.Create_Async(
            dbCon: dbCon,
            simpleUserId: newUserId,
            userDefaultPostsContextId: userDefaultPostsContext.Id,
            userDefaultTermId: userTerm.Id
        );

        return (userTerm, userDefaultPostsContext);
    }
}
