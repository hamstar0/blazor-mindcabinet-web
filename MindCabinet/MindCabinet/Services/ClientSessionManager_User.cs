using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;

namespace MindCabinet.Services;



public partial class ClientSessionManager {
    /**
     * @return `true` if session is a valid user.
     */
    private async Task<bool> LoadUserOfSession_Async(
                IDbConnection dbCon,
                ServerDataAccess_SimpleUsers userDataSrc,
                string sessId,
                string ip ) {
        if( this.UserOfSession is not null ) {
            if( this.CurrentSessionId != sessId ) {
                throw new Exception( "shit be whack, yo" ); //TODO
            }
            
            return true;
        }

        this.UserOfSession = (await userDataSrc.GetSimpleUserBySession_Async( dbCon, sessId, ip, true ))?
            .CreateDataObject();

        return this.UserOfSession is not null;
    }


    public async Task Visit_Async( IDbConnection dbCon, ServerDataAccess_SimpleUserSessions sessionsDataSrc ) {
        await sessionsDataSrc.VisitSimpleUserSession_Async( dbCon, this );
    }


    public async Task LogoutUser_Async( IDbConnection dbCon, ServerDataAccess_SimpleUserSessions sessionsDataSrc ) {
        this.UserOfSession = null;
        
        if( this.CurrentSessionId is null ) {
            return;
        }

        await sessionsDataSrc.RemoveSessionById_Async( dbCon, this.CurrentSessionId );
        
        //this.CurrentSessionId = null;
    }
}
