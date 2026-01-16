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

namespace MindCabinet.Client.Services.DataProcessors;



public class ContextPostsSupplier {
    [Inject]
    private ILogger<ContextPostsSupplier> Logger { get; set; } = null!;

    [Inject]
    private ClientSessionData SessionData { get; set; } = null!;


    [Inject]
    private ClientDataAccess_UserContext UserContextsData { get; set; } = null!;
    
    [Inject]
    private ClientDataAccess_PrioritizedPosts PostsData { get; set; } = null!;

    private int CurrentPostsPage = 0;

    private int CurrentPostsPerPage = 10;



    public ContextPostsSupplier( ClientDataAccess_PrioritizedPosts postsData ) {
        this.PostsData = postsData;
    }


    public async Task<IEnumerable<SimplePostObject>> GetPosts_Async(
                long userContextId,
                long[] additionalTagIds ) {
        IEnumerable<UserContextObject> ctxs = await this.UserContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Params{
                Ids = [ userContextId ]
            }
        );
        UserContextObject? currCtx = ctxs.FirstOrDefault();
        if( currCtx is null ) {
            // TODO log missing current context
            throw new Exception( "Missing current context from server" );
            //return [];
        }

        // long[] anyTagsIds = currCtx.Entries
        //     .Where( e => !e.IsRequired )
        //     .Select( e => e.Term.Id )
        //     .ToArray();
        // long[] allTagsIds = currCtx.Entries
        //     .Where( e => e.IsRequired )
        //     .Select( e => e.Term.Id )
        //     .ToArray();

        IEnumerable<SimplePostObject> posts = await this.PostsData.GetByCriteria_Async(
            new ClientDataAccess_PrioritizedPosts.GetByCriteria_Params(
                userContextId: userContextId,
                bodyPattern: null,
                additionalTagIds: additionalTagIds,
                sortAscendingByDate: true,
                pageNumber: this.CurrentPostsPage,
                postsPerPage: this.CurrentPostsPerPage
            )
        );

        var postPriorities = posts.Select( post => new KeyValuePair<long, double?>(
            post.Id,
            this.GetPriority( currCtx!, post )
        ) ).ToDictionary( kvp => kvp.Key, kvp => kvp.Value );

        if( postPriorities.ContainsValue(null) ) {
            this.Logger.LogWarning(
                $"Some posts returned for context {userContextId} have null priority."
            );
        }
        
        return posts
            .Where( post => postPriorities[post.Id] is not null )
            .OrderBy( post => postPriorities[post.Id] );
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
