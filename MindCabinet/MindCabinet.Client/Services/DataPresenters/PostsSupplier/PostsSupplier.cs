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



public partial class PostsSupplier(
                ILogger logger,
                LocalClientSessionManager mySessionMngr,
                ClientDataAccess_PrioritizedPosts postsDataSrc,
                PostsContextObject postsContext
            ) {
    private ILogger Logger = logger;

    private LocalClientSessionManager MySessionMngr = mySessionMngr;
    
    private ClientDataAccess_PrioritizedPosts PrioritizedPostsDataSrc = postsDataSrc;
    
    private PostsContextObject PostsContext = postsContext;


    private int CurrentPage = 0;

    private int MaxPostsPerPage = 10;

    private bool SortAscendingByDate = false;



    public async Task<IEnumerable<SimplePostObject>> GetPosts_Async(
                ClientDataAccess_Terms termsDataSrc,
                string? searchTerm,
                TermId[] addedFilterTagIds ) {
        // PostsContextObject? postsContext = this.MySessionMngr.GetCurrentContext();

        IEnumerable<SimplePostObject.Raw> postsRaw = await this.PrioritizedPostsDataSrc.GetByCriteriaForCurrentUser_Async(
            new ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params(
                postsContextId: this.PostsContext.Id,
                bodyPattern: searchTerm,
                additionalTagIds: addedFilterTagIds,
                sortAscendingByDate: this.SortAscendingByDate,
                pageNumber: this.CurrentPage,
                postsPerPage: this.MaxPostsPerPage
            )
        );

        Dictionary<SimplePostId, double?> postPriorities = postsRaw
            .Select( post => new KeyValuePair<SimplePostId, double?>(
                key: post.Id,
                value: this.GetPriority(this.PostsContext, post)
            ) ).ToDictionary( kvp => kvp.Key, kvp => kvp.Value );

        if( postPriorities.ContainsValue(null) ) {
            this.Logger.LogWarning(
                $"Some posts returned for context {this.PostsContext.ToString()} have null priority."
            );
        }

        SimplePostObject[] posts = new SimplePostObject[ postsRaw.Count() ];
        for( int i = 0; i < postsRaw.Count(); i++ ) {
            posts[i] = await ClientDataAccess_SimplePosts.ConvertRawToDataObject_Async( termsDataSrc, postsRaw.ElementAt(i) );
        }

        return posts
            .Where( post => postPriorities[post.Id] is not null )
            .OrderBy( post => postPriorities[post.Id] );
    }

    public async Task<int> GetPostCount_Async(
                string? searchTerm,
                TermId[] addedFilterTagIds ) {
        // PostsContextObject? currCtx = this.MySessionMngr.GetCurrentContext();

        int totalPosts = await this.PrioritizedPostsDataSrc.GetCountByCriteria_Async(
            new ClientDataAccess_PrioritizedPosts.IAPI.GetByCriteria_Params(
                postsContextId: this.PostsContext.Id,
                bodyPattern: searchTerm,
                additionalTagIds: addedFilterTagIds,
                sortAscendingByDate: this.SortAscendingByDate,
                pageNumber: 0,
                postsPerPage: -1
            )
        );

        return totalPosts;
    }


    public double? GetPriority( PostsContextObject ctx, SimplePostObject.Raw post ) {
        double totalPriority = 0;
        int matchedCount = 0;

        foreach( PostsContextTermEntryObject entry in ctx.Entries ) {
            if( post.TagsTermIdSet.FirstOrDefault(tid => tid == entry.Term.Id) != 0 ) {
                matchedCount++;
                totalPriority += entry.Priority;
            } else if( entry.IsRequired ) {
                return null;
            }
        }

        return totalPriority;
        // return matchedCount > 0
        //     ? totalPriority
        //     : null;
    }
}
