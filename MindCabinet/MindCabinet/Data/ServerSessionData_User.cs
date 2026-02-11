using MindCabinet.Data.DataAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;
using System.Security.Cryptography;

namespace MindCabinet.Data;



public partial class ServerSessionData {
    public SimpleUserObject? User { get; private set; }



    /**
     * @return `true` if session is a valid user.
     */
    private async Task<bool> LoadUserOfSession_Async(
                IDbConnection dbCon,
                ServerDataAccess_SimpleUsers userData,
                string sessId ) {
        string ip = this.HttpContext.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";
        if( string.IsNullOrEmpty(ip) ) {
            throw new Exception( "Who are you?" );
        }

        if( this.User is not null ) {
            if( this.IpAddress != ip ) {
                throw new Exception( "Hax!" );  //TODO
            }
            if( this.SessionId != sessId ) {
                throw new Exception( "shit be whack, yo" ); //TODO
            }
            
            return true;
        }

        this.User = await userData.GetSimpleUserBySession_Async( dbCon, sessId, ip );

        return this.User is not null;
    }


    public async Task Visit_Async( IDbConnection dbCon, ServerDataAccess_SimpleUsers_Sessions sessionsData ) {
        await sessionsData.VisitSimpleUserSession_Async( dbCon, this );
    }
}
