using Dapper;
using MindCabinet.Client.Data;
using MindCabinet.Shared.DataEntries;
using System.Data;


namespace MindCabinet.Data;


public partial class ServerDataAccess {
    public class PostEntryData {
        public long Id;
        public DateTime Created;
        public string Body = "";
        public int TermSetId;


        public async Task<PostEntry> CreatePost_Async( IDbConnection dbCon, ServerDataAccess data ) {
            return new PostEntry(
                id: this.Id,
                created: this.Created,
                body: this.Body,
                tags: (await data.GetTermSet_Async(dbCon, this.TermSetId)).ToList()
            );
        }
    }



    public async Task<bool> InstallPosts_Async( IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE Posts (
                Id BIGINT NOT NULL IDENTITY(1, 1) PRIMARY KEY CLUSTERED,
                Created DATETIME2(2) NOT NULL,
                Body VARCHAR(512) NOT NULL,
                TermSetId INT NOT NULL
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
                Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(25),
               TermSetId = await this.CreateTermSet_Async(dbConnection, term1.Term, term3.Term)
            },
            new {
                Body = "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(24),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(23),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(21),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(19),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(18),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(15),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?",
                Created = DateTime.UtcNow - TimeSpan.FromHours(11),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term2.Term)
            },
            new {
                Body = "Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
                Created = DateTime.UtcNow - TimeSpan.FromHours(10),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term2.Term)
            },
            new {
                Body = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(9),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                Body = "Et harum quidem rerum facilis est et expedita distinctio.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(8),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                Body = "Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(7),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                Body = "Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(5),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                Body = "Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(3),
                TermSetId = await this.CreateTermSet_Async(dbConnection, term2.Term, term3.Term)
            },
        };

        var sql = @"INSERT INTO Posts (Body, Created, TermSetId)
                    VALUES (@Body, @Created, @TermSetId)";
        int rowsAffected = await dbConnection.ExecuteAsync( sql, fillerPosts );

        return true;
    }



    public async Task<IEnumerable<PostEntry>> GetPostsByCriteria_Async(
                IDbConnection dbCon,
                ClientDataAccess.GetPostsByCriteriaParams parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return Enumerable.Empty<PostEntry>();
        }
        
        string sql = @"SELECT * FROM Posts AS MyPosts
                WHERE MyPosts.Body LIKE @Body";
        var sqlParams = new Dictionary<string, object> { { "Body", $"%{parameters.BodyPattern}%" } };

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

        sql += $" ORDER BY Created {(parameters.SortAscendingByDate ? "ASC" : "DESC")}";
        if( parameters.PostsPerPage > 0 ) {
            sql += @" OFFSET @Offset ROWS
                    FETCH NEXT @Quantity ROWS ONLY;";
            sqlParams["@Offset"] = parameters.PageNumber * parameters.PostsPerPage;
            sqlParams["@Quantity"] = parameters.PostsPerPage;
        }

        //

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
                WHERE MyPosts.Body LIKE @Body";
        var sqlParams = new Dictionary<string, object> { { "Body", $"%{parameters.BodyPattern}%" } };

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
            @"INSERT INTO Posts (Created, Body, TermSetId) 
                VALUES (@Created, @Body, @TermSetId)
                OUTPUT INSERTED.Id",
            new {
                Created = now,
                Body = parameters.Body,
                TermSetId = await this.CreateTermSet_Async( dbCon, parameters.Tags.ToArray() ),
            }
        );

        var newTerm = new PostEntry(
            id: newPostId,
            created: now,
            body: parameters.Body,
            tags: parameters.Tags
        );

        return newTerm;
    }
}
