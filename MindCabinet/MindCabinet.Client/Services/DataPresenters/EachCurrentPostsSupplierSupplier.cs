using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;

namespace MindCabinet.Client.Services.DataPresenters;



public partial class EachCurrentPostsSupplierSupplier(
                ILogger<EachCurrentPostsSupplierSupplier> logger,
                ClientDataAccess_Terms termsDataSrc,
                ClientDataAccess_PostsQuery postsQueryDataSrc,
                ClientDataAccess_PrioritizedPosts prioritizedPostsDataSrc
            ) : IClientDataProcessors {
    private ILogger<EachCurrentPostsSupplierSupplier> Logger = logger;

    private ClientDataAccess_Terms TermsDataSrc = termsDataSrc;

    private ClientDataAccess_PostsQuery PostsQueryDataSrc = postsQueryDataSrc;

    private ClientDataAccess_PrioritizedPosts PrioritizedPostsDataSrc = prioritizedPostsDataSrc;



    public async Task<IEnumerable<PostsSupplier>> GetPostsSuppliers_Async() {
        IEnumerable<PostsQueryObject.Raw> currContextRaws = (await this.PostsQueryDataSrc.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_PostsQuery.IAPI.GetByCriteria_Params { }
        )).Queries;

        List<PostsQueryObject> currContexts = new List<PostsQueryObject>( currContextRaws.Count() );
        foreach( PostsQueryObject.Raw raw in currContextRaws ) {
            currContexts.Add( await ClientDataAccess_PostsQuery.ConvertRawToDataObject_Async(this.TermsDataSrc, raw) );
        }

        return currContexts.Select( context => new PostsSupplier(
            logger: this.Logger,
            postsDataSrc: this.PrioritizedPostsDataSrc,
            postsQuery: context
        ) );
    }
}
