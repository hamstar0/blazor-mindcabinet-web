using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;

namespace MindCabinet.Data;



public partial class ServerSessionData {
    /**
     * @return `true` if session is a valid user.
     */
    private async Task<bool> LoadUserOfSession_Async(
                IDbConnection dbCon,
                ServerDataAccess_SimpleUsers userData,
                string sessId,
                string ip ) {
        if( this.UserOfSession is not null ) {
            if( this.SessionId != sessId ) {
                throw new Exception( "shit be whack, yo" ); //TODO
            }
            
            return true;
        }

        this.UserOfSession = await userData.GetSimpleUserBySession_Async( dbCon, sessId, ip );

        return this.UserOfSession is not null;
    }


    public async Task Visit_Async( IDbConnection dbCon, ServerDataAccess_SimpleUserSessions sessionsData ) {
        await sessionsData.VisitSimpleUserSession_Async( dbCon, this );
    }


    public async Task LogoutUser_Async( IDbConnection dbCon, ServerDataAccess_SimpleUserSessions sessionsData ) {
        this.UserOfSession = null;
        
        if( this.SessionId is null ) {
            return;
        }

        await sessionsData.RemoveSimpleUserBySession_Async( dbCon, this.SessionId );
        
        //this.SessionId = null;
    }
}
