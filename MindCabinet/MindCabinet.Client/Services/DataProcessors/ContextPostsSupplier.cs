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


    public async Task<IEnumerable<SimplePostObject>> GetPosts_Async( long userContextId ) {
        long? currCtxId = SessionData.GetCurrentContextById();
        if( currCtxId is null ) {
            return [];
        }

        IEnumerable<UserContextObject> ctxs = await this.UserContextsData.GetForCurrentUserByCriteria_Async(
            new ClientDataAccess_UserContext.GetForCurrentUserByCriteria_Params{
                Ids = [ currCtxId.Value ]
            }
        );
        UserContextObject? currCtx = ctxs.FirstOrDefault();
        if( currCtx is null ) {
            // TODO log missing current context
            throw new Exception( "Missing current context from server" );
            //return [];
        }

        TermObject[] anyTags = currCtx.Entries
            .Where( e => !e.IsRequired )
            .Select( e => e.Term )
            .ToArray();
        TermObject[] allTags = currCtx.Entries
            .Where( e => e.IsRequired )
            .Select( e => e.Term )
            .ToArray();

        IEnumerable<SimplePostObject> posts = await this.PostsData.GetByCriteria_Async(
            new ClientDataAccess_PrioritizedPosts.GetByCriteria_Params(
                bodyPattern: null,
                anyTags: anyTags,
                allTags: allTags,
                sortAscendingByDate: true,
                pageNumber: this.CurrentPostsPage,
                postsPerPage: this.CurrentPostsPerPage
            )
        );
        
        return posts.OrderBy( post => this.GetPriority(currCtx!, post) );
    }

    public double GetPriority( UserContextObject ctx, SimplePostObject post ) {
        double priority = 0.0;

        HashSet<long> postTagIds = post.Tags?.Select( t => t.Id ).ToHashSet() ?? new HashSet<long>();

        foreach( UserContextTermEntryObject entry in ctx.Entries ) {
            if( postTagIds.Contains(entry.Term.Id) ) { f
                priority += entry.Priority;
            }
        }

        return priority;
    }
}
