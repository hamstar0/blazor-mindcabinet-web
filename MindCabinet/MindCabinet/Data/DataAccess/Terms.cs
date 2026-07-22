using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;
using System;
using System.Data;
using System.Text;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_Terms(
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    private static readonly SimpleCache<TermId, TermObject.Raw?> Cache_ById = new( refreshExpiryOnGet: true );



    private readonly StaticServerSettings ServerSettings = serverSettings;
}
