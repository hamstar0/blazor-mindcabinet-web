using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;

namespace MindCabinet.Client.Services.DataPresenters;



public partial class EachCurrentPostsSupplierSupplier(
                ILogger<EachCurrentPostsSupplierSupplier> logger,
                ClientDataAccess_Terms termsDataSrc,
                LocalClientSessionManager mySessionMngr,
                ClientDataAccess_PostsContext postsContextsDataSrc,
                ClientDataAccess_PrioritizedPosts prioritizedPostsDataSrc
            ) : IClientDataProcessors {
    private ILogger<EachCurrentPostsSupplierSupplier> Logger = logger;

    private LocalClientSessionManager MySessionMngr = mySessionMngr;

    private ClientDataAccess_Terms TermsDataSrc = termsDataSrc;

    private ClientDataAccess_PostsContext PostsContextsDataSrc = postsContextsDataSrc;

    private ClientDataAccess_PrioritizedPosts PrioritizedPostsDataSrc = prioritizedPostsDataSrc;


    private List<PostsSupplier> Suppliers_Cache = new();



    public async Task<IEnumerable<PostsSupplier>> GetPostsSuppliers_Async() {
        IEnumerable<PostsContextObject.Raw> currContextRaws = (await this.PostsContextsDataSrc.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_PostsContext.IAPI.GetByCriteria_Params { }
        )).Contexts;

        List<PostsContextObject> currContexts = new List<PostsContextObject>( currContextRaws.Count() );
        foreach( PostsContextObject.Raw raw in currContextRaws ) {
            currContexts.Add( await ClientDataAccess_PostsContext.ConvertRawToDataObject_Async(this.TermsDataSrc, raw) );
        }

        return currContexts.Select( context => new PostsSupplier(
            logger: this.Logger,
            mySessionMngr: this.MySessionMngr,
            postsDataSrc: this.PrioritizedPostsDataSrc,
            postsContext: context
        ) );
    }
}
