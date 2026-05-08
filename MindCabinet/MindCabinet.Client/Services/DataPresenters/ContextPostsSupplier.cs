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



public partial class ContextPostsSupplier(
            ILogger<ContextPostsSupplier> logger,
            ClientSessionManager sessionData,
            ClientDataAccess_PostsContext postsContextsData,
            ClientDataAccess_PrioritizedPosts postsData
        ) : IClientDataProcessors {
    private ILogger<ContextPostsSupplier> Logger = logger;

    private ClientSessionManager SessionData = sessionData;

    private ClientDataAccess_PostsContext PostsContextsData = postsContextsData;
    
    private ClientDataAccess_PrioritizedPosts PrioritizedPostsData = postsData;


    private int CurrentPage = 0;

    private int MaxPostsPerPage = 10;

    private bool SortAscendingByDate = false;



    public async Task<IEnumerable<SimplePostObject>> GetCurrentContextPosts_Async(
                ClientDataAccess_Terms termsData,
                string? searchTerm,
                TermId[] addedFilterTagIds ) {
        PostsContextObject? postsContext = this.SessionData.GetCurrentContext();
        if( postsContext is null ) {
            return [];
        }

        IEnumerable<SimplePostObject.Raw> postsRaw = await this.PrioritizedPostsData.GetByCriteria_Async(
            new ClientDataAccess_PrioritizedPosts.GetByCriteria_Params(
                postsContextId: postsContext.Id,
                bodyPattern: searchTerm,
                additionalTagIds: addedFilterTagIds,
                sortAscendingByDate: true,
                pageNumber: this.CurrentPage,
                postsPerPage: this.MaxPostsPerPage
            )
        );

        var postPriorities = postsRaw.Select( post => new KeyValuePair<SimplePostId, double?>(
            key: post.Id,
            value: this.GetPriority(postsContext, post)
        ) ).ToDictionary( kvp => kvp.Key, kvp => kvp.Value );

        if( postPriorities.ContainsValue(null) ) {
            this.Logger.LogWarning(
                $"Some posts returned for context {postsContext.ToString()} have null priority."
            );
        }

        SimplePostObject[] posts = new SimplePostObject[ postsRaw.Count() ];
        for( int i = 0; i < postsRaw.Count(); i++ ) {
            posts[i] = await ClientDataAccess_SimplePosts.ConvertRawToDataObject_Async( termsData, postsRaw.ElementAt(i) );
        }

        return posts
            .Where( post => postPriorities[post.Id] is not null )
            .OrderBy( post => postPriorities[post.Id] );
    }

    public async Task<int> GetCurrentContextPostCount_Async(
                string? searchTerm,
                TermId[] addedFilterTagIds ) {
        PostsContextObject? currCtx = this.SessionData.GetCurrentContext();
        if( currCtx is null ) {
            return 0;
        }

        int totalPosts = await this.PrioritizedPostsData.GetCountByCriteria_Async(
            new ClientDataAccess_PrioritizedPosts.GetByCriteria_Params(
                postsContextId: currCtx.Id,
                bodyPattern: searchTerm,
                additionalTagIds: addedFilterTagIds,
                sortAscendingByDate: true,
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

        return matchedCount > 0
            ? totalPriority
            : null;
    }
}
