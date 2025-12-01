using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimplePosts {
    public class SimplePostEntryData {
        public long Id;
        public DateTime Created;
        public string Body = "";
        public int TermSetId;


        public async Task<SimplePostObject> CreateSimplePost_Async(
                    IDbConnection dbCon,
                    ServerDataAccess_Terms termsData,
                    ServerDataAccess_Terms_Sets termSetsData ) {
            return new SimplePostObject(
                id: this.Id,
                created: this.Created,
                body: this.Body,
                tags: (await termSetsData.GetTermSet_Async(dbCon, termsData, this.TermSetId)).ToList()
            );
        }
    }

    //



    public async Task<bool> Install_Async(
                IDbConnection dbConnection, 
                ServerDataAccess_Terms termsData,
                ServerDataAccess_Terms_Sets termSetsData,
                long defaultUserId ) {
        await dbConnection.ExecuteAsync( @"
            CREATE TABLE SimplePosts (
                Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                Created DATETIME(2) NOT NULL,
                Modified DATETIME(2) NOT NULL,
                SimpleUserId BIGINT NOT NULL,
                Body MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                TermSetId INT NOT NULL,
                CONSTRAINT FK_PostsUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES SimpleUsers(Id)
            );"
            //    ON DELETE CASCADE
            //    ON UPDATE CASCADE
        );

        //

        await this.InstallSamples_Async( dbConnection, termsData, termSetsData, defaultUserId );

        return true;
    }

    //



    private (string sql, IDictionary<string, object> sqlParams) GetByCriteriaSql(
                ClientDataAccess_SimplePosts.GetByCriteria_Params parameters,
                bool countOnly ) {
        bool hasWhere = false;
        string sql = $"SELECT {(countOnly ? "COUNT(*)" : "*")} FROM SimplePosts AS MyPosts ";
        var sqlParams = new Dictionary<string, object>();

        if( !string.IsNullOrEmpty(parameters.BodyPattern) ) {
            string body = parameters.BodyPattern.Replace( "%", "\\%" );
            body = body.Replace( "_", "\\_" );
            //body = body.Replace( "[", "\\[" );

            // sql += "WHERE MyPosts.Body LIKE REPLACE(REPLACE(REPLACE(@Body, '[', '[[]'), '_', '[_]'), '%', '[%]')";
            sql += "\nWHERE MyPosts.Body LIKE @Body ESCAPE '\\\\'";
            sqlParams["@Body"] = new DbString { Value = $"%{body}%", IsAnsi = true };
            hasWhere = true;
        }

        if( parameters.Tags.Count > 0 ) {
            sql += hasWhere ? "AND" : "WHERE";
            sql += @"
            (
                (
                    (SELECT (@Tags)) EXCEPT (
                        SELECT MyTerms.Id FROM Terms AS MyTerms
                        INNER JOIN TermSet AS MyTermSet ON (MyTermSet.TermId = MyTerms.Id)
                        WHERE MyTermSet.SetId = MyPosts.TermSetId
                    )
                ) IS NULL
            )";
            sqlParams["@Tags"] = parameters.Tags
                .Select( t => t.Id )
                .ToList();
            //      ) ALL (";
            //int i = 1;
            //foreach( TermEntry tag in parameters.Tags ) {
            //    if( i > 1 ) { sql += ", "; }
            //    sql += "@Tag"+i;
            //    sqlParams[ "Tag"+i ] = tag.Id!;
            //    i++;
            //}
            //sql += ")";
            hasWhere = true;
        }

        if( !countOnly ) {
            sql += $"\n ORDER BY Created {(parameters.SortAscendingByDate ? "ASC" : "DESC")}";
        }

        if( parameters.PostsPerPage > 0 ) {
            // FETCH NEXT @Quantity ROWS ONLY;";
            // LIMIT @Offset ROWS OFFSET @Quantity ROWS ONLY;";
            sql += $"\n LIMIT @Offset, @Quantity";
            sqlParams["@Offset"] = parameters.PageNumber * parameters.PostsPerPage;
            sqlParams["@Quantity"] = parameters.PostsPerPage;
        }

        sql += ";";
        return (sql, sqlParams);
    }

    public async Task<IEnumerable<SimplePostObject>> GetByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_Terms_Sets termSetsData,
                ClientDataAccess_SimplePosts.GetByCriteria_Params parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return Enumerable.Empty<SimplePostObject>();
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql( parameters, false );

// this.Logger.LogInformation( "Executing SQL: {Sql} with params {Params}", sql, sqlParams );
        IEnumerable<SimplePostEntryData> posts = await dbCon.QueryAsync<SimplePostEntryData>(
            sql, new DynamicParameters( sqlParams )
        );

        var postList = new List<SimplePostObject>( posts.Count() );

        foreach( SimplePostEntryData post in posts ) {
            postList.Add( await post.CreateSimplePost_Async(dbCon, termsData, termSetsData) );
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

    public async Task<int> GetCountByCriteria_Async(
                IDbConnection dbCon,
                ClientDataAccess_SimplePosts.GetByCriteria_Params parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return 0;
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql( parameters, true );

        return await dbCon.QuerySingleAsync<int>( sql, new DynamicParameters(sqlParams) );

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


	public async Task<SimplePostObject> Create_Async(
                IDbConnection dbCon,
                long simpleUserId,
                ServerDataAccess_Terms_Sets termSetsData,
                ServerDataAccess_UserTermsHistory termHistoryData,
                ClientDataAccess_SimplePosts.Create_Params parameters,
                bool skipHistory ) {
        DateTime now = DateTime.UtcNow;

        long newPostId = await dbCon.ExecuteScalarAsync<long>(   //ExecuteAsync + ExecuteScalarAsync?
            @"INSERT INTO SimplePosts (Created, Modified, Body, TermSetId) 
                VALUES (@Created, @Created, @Body, @TermSetId);
            SELECT LAST_INSERT_ID();",
            new {
                Created = now,
                Body = new DbString { Value = parameters.Body, IsAnsi = true },
                TermSetId = await termSetsData.Create_Async( dbCon, parameters.Tags.ToArray() ),
            }
        );

        var newTerm = new SimplePostObject(
            id: newPostId,
            created: now,
            body: parameters.Body,
            tags: parameters.Tags
        );

        if( !skipHistory ) {
            await Task.WhenAll( parameters.Tags.Select( tag =>
                termHistoryData.AddTerm_Async(
                    dbCon,
                    simpleUserId,
                    new ClientDataAccess_UserTermsHistory.AddTermsForCurrentUser_Params(
                        termId: tag.Id
                    )
                )
            ) );
        }

        return newTerm;
    }
}
