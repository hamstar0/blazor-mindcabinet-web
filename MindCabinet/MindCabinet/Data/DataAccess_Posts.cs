using Dapper;
using MindCabinet.Client.Data;
using MindCabinet.Shared.DataEntries;
using System.Data;


namespace MindCabinet.Data;


public partial class ServerDataAccess {
    public class PostEntryData {
        public long Id;
        public DateTime When;
        public string Body = "";
        public int TermSetId;


        public async Task<PostEntry> CreatePost_Async( IDbConnection dbCon, ServerDataAccess data ) {
            return new PostEntry(
                id: this.Id,
                when: this.When,
                body: this.Body,
                tags: (await data.GetTermSet_Async(dbCon, this.TermSetId)).ToList()
            );
        }
    }



    public async Task<bool> InstallPosts_Async( IDbConnection dbConnection ) {
        //PostID BIGINT PRIMARY KEY CLUSTERED,
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE Posts (
                Id BIGINT NOT NULL PRIMARY KEY CLUSTERED,
                When DATETIME2 NOT NULL,
                Body VARCHAR NOT NULL,
                TermSetId BIGINT NOT NULL,
                CONSTRAINT FK_TermSetId FOREIGN KEY (TermSetId)
                    REFERENCES TermSet(SetId)
            );"
        //    ON DELETE CASCADE
        //    ON UPDATE CASCADE
        );

        //

        ClientDataAccess.CreateTermReturn term1 = await this.CreateTerm_Async(
            dbConnection,
            new ClientDataAccess.CreateTermParams("Term1", null, null)
        );
        ClientDataAccess.CreateTermReturn term2 = await this.CreateTerm_Async(
            dbConnection,
            new ClientDataAccess.CreateTermParams("Term2", null, null)
        );
        ClientDataAccess.CreateTermReturn term3 = await this.CreateTerm_Async(
            dbConnection,
            new ClientDataAccess.CreateTermParams("Term3", null, null)
        );

        var fillerPosts = new List<object>() {
            new {
                PostBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term1.Term, term3.Term)
            },
            new {
                PostBody = "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                PostBody = "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                PostBody = "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                PostBody = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                PostBody = "Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                PostBody = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                PostBody = "Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term2.Term)
            },
            new {
                PostBody = "Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term2.Term)
            },
            new {
                PostBody = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                PostBody = "Et harum quidem rerum facilis est et expedita distinctio.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                PostBody = "Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                PostBody = "Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                PostBody = "Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.",
                TagIDs = await this.CreateTermSet_Async(dbConnection, term2.Term, term3.Term)
            },
        };

        var sql = "INSERT INTO Posts (PostBody, TagIDs) VALUES (@PostBody, @TagIDs)";
        var rowsAffected = dbConnection.Execute( sql, fillerPosts );

        return true;
    }



    public async Task<IEnumerable<PostEntry>> GetPostsByCriteria_Async(
                IDbConnection dbCon,
                ClientDataAccess.GetPostsByCriteriaParams parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return Enumerable.Empty<PostEntry>();
        }
        
        string sql = @"SELECT * FROM Posts AS MyPosts
                WHERE MyPosts.PostBody LIKE @BodyPattern";
        var sqlParams = new Dictionary<string, object> { { "BodyPattern", $"%{parameters.BodyPattern}%" } };

        if( parameters.Tags.Count > 0 ) {
            sql += @" AND (
                        SELECT MyTerms.Id FROM Terms AS MyTerms
                        INNER JOIN TermSet AS MyTermSet ON (MyTermSet.TermId = MyTerms.Id)
                        WHERE MyTermSet.Id = MyPosts.TermSetId
                    ) ALL (";

            int i = 1;
            foreach( TermEntry tag in parameters.Tags ) {
                if( i > 1 ) { sql += ", "; }
                sql += "@Tag"+i;
                sqlParams[ "Tag"+i ] = tag.Id!;
                i++;
            }
            sql += ")";
        }

        IEnumerable<PostEntryData> posts = await dbCon.QueryAsync<PostEntryData>(
            sql, new DynamicParameters( sqlParams )
        );

        //

        IList<PostEntry> postList = new List<PostEntry>( posts.Count() );

        foreach( PostEntryData post in posts ) {
            postList.Add( await post.CreatePost_Async(dbCon, this) );
        }

        return postList;

        //var filteredPosts = this.Posts.Values
		//	.Where( p => p.Test(parameters.BodyPattern, parameters.Tags) );
		//var orderedPosts = filteredPosts
		//	.OrderBy( p => parameters.SortAscendingByDate ? p.Timestamp : -p.Timestamp );
        //
        //IEnumerable<PostEntry> posts = orderedPosts;
        //
        //if( parameters.PostsPerPage > 0 ) {
        //    posts = orderedPosts
        //        .Skip( parameters.PageNumber * parameters.PostsPerPage )
        //        .Take( parameters.PostsPerPage );
        //}
        //return posts;
	}

    public async Task<int> GetPostCountByCriteria_Async(
                IDbConnection dbCon,
                ClientDataAccess.GetPostsByCriteriaParams parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return 0;
        }

        string sql = @"SELECT COUNT(*) FROM Posts AS MyPosts
                WHERE MyPosts.PostBody LIKE @BodyPattern";
        var sqlParams = new Dictionary<string, object> { { "BodyPattern", $"%{parameters.BodyPattern}%" } };

        if( parameters.Tags.Count > 0 ) {
            sql += @" AND (
                        SELECT MyTerms.Id FROM Terms AS MyTerms
                        INNER JOIN TermSet AS MyTermSet ON (MyTermSet.TermId = MyTerms.Id)
                        WHERE MyTermSet.Id = MyPosts.TermSetId
                    ) ALL (";

            int i = 1;
            foreach( TermEntry tag in parameters.Tags ) {
                if( i > 1 ) { sql += ", "; }
                sql += "@Tag" + i;
                sqlParams["Tag" + i] = tag.Id!;
                i++;
            }
            sql += ")";
        }

        return await dbCon.QuerySingleAsync<int>(
            sql, new DynamicParameters( sqlParams )
        );

        //var filteredPosts = this.Posts.Values
        //    .Where( p => p.Test( parameters.BodyPattern, parameters.Tags ) );
        //
        //IEnumerable<PostEntry> posts = filteredPosts;
        //
        //if( parameters.PostsPerPage > 0 ) {
        //    posts = filteredPosts
        //        .Skip( parameters.PageNumber * parameters.PostsPerPage )
        //        .Take( parameters.PostsPerPage );
        //}
        //
        //return posts.Count();
    }


	public async Task<PostEntry> CreatePost_Async(
                IDbConnection dbCon,
                ClientDataAccess.CreatePostParams parameters ) {
        DateTime now = DateTime.UtcNow;

        int newPostId = await dbCon.QuerySingleAsync(
            @"INSERT INTO Posts (When, Body, TermSetId) 
                VALUES (@When, @Body, @TermSetId)
                OUTPUT INSERTED.Id",
            new {
                When = now,
                Body = parameters.Body,
                TermSetId = await this.CreateTermSet_Async( dbCon, parameters.Tags.ToArray() ),
            }
        );

        var newTerm = new PostEntry(
            id: newPostId,
            when: now,
            body: parameters.Body,
            tags: parameters.Tags
        );

        return newTerm;

        //long id = this.CurrentPostId++;
        //var post = new PostEntry(
        //	id: id,
        //	timestamp: DateTime.Now.Ticks,
        //	body: parameters.Body,
        //	tags: parameters.Tags
        //);
        //
        //this.Posts[id] = post;
        //
        //return post;
    }
}
