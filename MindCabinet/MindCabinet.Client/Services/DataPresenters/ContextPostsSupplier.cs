using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.UserContext;

namespace MindCabinet.Client.Services.DataPresenters;



public partial class ContextPostsSupplier(
            ILogger<ContextPostsSupplier> logger,
            ClientSessionData sessionData,
            ClientDataAccess_UserContext userContextsData,
            ClientDataAccess_PrioritizedPosts postsData
        ) : IClientDataProcessors {
    private ILogger<ContextPostsSupplier> Logger = logger;

    private ClientSessionData SessionData = sessionData;

    private ClientDataAccess_UserContext UserContextsData = userContextsData;
    
    private ClientDataAccess_PrioritizedPosts PostsData = postsData;


    private int CurrentPage = 0;

    private int MaxPostsPerPage = 10;

    private bool SortAscendingByDate = false;



    public async Task<IEnumerable<SimplePostObject>> GetCurrentContextPosts_Async(
                string? searchTerm,
                long[] addedFilterTagIds ) {
        UserContextObject? userContext = this.SessionData.GetCurrentContext();
        if( userContext is null ) {
            return [];
        }

        IEnumerable<SimplePostObject> posts = await this.PostsData.GetByCriteria_Async(
            new ClientDataAccess_PrioritizedPosts.GetByCriteria_Params(
                userContextId: userContext.Id,
                bodyPattern: searchTerm,
                additionalTagIds: addedFilterTagIds,
                sortAscendingByDate: true,
                pageNumber: this.CurrentPage,
                postsPerPage: this.MaxPostsPerPage
            )
        );

        var postPriorities = posts.Select( post => new KeyValuePair<long, double?>(
            key: post.Id,
            value: this.GetPriority(userContext, post)
        ) ).ToDictionary( kvp => kvp.Key, kvp => kvp.Value );

        if( postPriorities.ContainsValue(null) ) {
            this.Logger.LogWarning(
                $"Some posts returned for context {userContext.ToString()} have null priority."
            );
        }
        
        return posts
            .Where( post => postPriorities[post.Id] is not null )
            .OrderBy( post => postPriorities[post.Id] );
    }

    public async Task<int> GetCurrentContextPostCount_Async(
                long[] addedFilterTagIds ) {
        UserContextObject? currCtx = this.SessionData.GetCurrentContext();
        if( currCtx is null ) {
            return 0;
        }

        int totalPosts = await this.PostsData.GetCountByCriteria_Async(
            new ClientDataAccess_PrioritizedPosts.GetByCriteria_Params(
                userContextId: currCtx.Id,
                bodyPattern: null,
                additionalTagIds: addedFilterTagIds,
                sortAscendingByDate: true,
                pageNumber: 0,
                postsPerPage: -1
            )
        );

        return totalPosts;
    }


    public double? GetPriority( UserContextObject ctx, SimplePostObject post ) {
        double totalPriority = 0;
        int matchedCount = 0;

        foreach( UserContextTermEntryObject entry in ctx.Entries ) {
            if( post.Tags.FirstOrDefault(t => t.Id == entry.Term.Id) is not null ) {
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
