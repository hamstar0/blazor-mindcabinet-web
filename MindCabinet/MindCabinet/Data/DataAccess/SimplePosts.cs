using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.DataObjects;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using System.Data;


namespace MindCabinet.Data.DataAccess;



public partial class ServerDataAccess_SimplePosts : IServerDataAccess {
    public const string TableName = "SimplePosts";

    public async Task<bool> Install_Async(
                IDbConnection dbConnection ) {
        await dbConnection.ExecuteAsync( $@"
            CREATE TABLE {TableName} (
                Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                Created DATETIME(2) NOT NULL,
                Modified DATETIME(2) NOT NULL,
                SimpleUserId BIGINT NOT NULL,
                Body MEDIUMTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
                 CONSTRAINT FK_{TableName}_SimpleUserId FOREIGN KEY (SimpleUserId)
                    REFERENCES {ServerDataAccess_SimpleUsers.TableName}(Id)
            );"
            //    ON DELETE CASCADE
            //    ON UPDATE CASCADE
        );

        return true;
    }

    public async Task<(bool success, TermObject.Raw sampleTerm)> Install_AfterUser_Async(
                IDbConnection dbConnection, 
                ServerDataAccess_Terms termsData,
                ServerDataAccess_SimplePostTags termSetsData,
                SimpleUserId defaultUserId,
                TermId defaultUserAsTermId ) {
        return await this.InstallSamples_Async( dbConnection, termsData, termSetsData, defaultUserId, defaultUserAsTermId );
    }

    //



    public async Task<SimplePostObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                SimplePostId id ) {
        if( id == 0 ) {
            throw new ArgumentException( "SimplePostId is not valid (must be non-zero)." );
        }

        SimplePostObject.Raw? postRaw = await dbCon.QuerySingleOrDefaultAsync<SimplePostObject.Raw>(
            $"SELECT * FROM {TableName} AS MyPosts WHERE Id = @Id",
            new { Id = (long)id }
        );

        return postRaw;
    }


    private (string sql, IDictionary<string, object> sqlParams) GetByCriteriaSql(
                ClientDataAccess_SimplePosts.GetByCriteria_Params parameters,
                bool countOnly ) {
        bool hasWhere = false;
        string sql = $"SELECT {(countOnly ? "COUNT(*)" : "*")} FROM {TableName} AS MyPosts ";
        var sqlParams = new Dictionary<string, object>();

        if( !string.IsNullOrEmpty(parameters.BodyPattern) ) {
            string body = parameters.BodyPattern.Replace( "%", "\\%" );
            body = body.Replace( "_", "\\_" );
            //body = body.Replace( "[", "\\[" );

            // sql += "WHERE MyPosts.Body LIKE REPLACE(REPLACE(REPLACE(@Body, '[', '[[]'), '_', '[_]'), '%', '[%]')";
            sql += "\nWHERE MyPosts.Body LIKE @Body ESCAPE '\\\\' ";
            sqlParams["@Body"] = new DbString { Value = $"%{body}%", IsAnsi = true };
            hasWhere = true;
        }

        if( parameters.AllTagIds.Length > 0 ) {
            sql += hasWhere ? "AND" : "WHERE";
            sql += $@" (
                (
                    (SELECT (@Tags)) EXCEPT (
                        SELECT MyTerms.Id FROM {ServerDataAccess_Terms.TableName} AS MyTerms
                        INNER JOIN {ServerDataAccess_SimplePostTags.TableName} AS MyTermSet ON (MyTermSet.TermId = MyTerms.Id)
                        WHERE MyTermSet.SetId = MyPosts.TermSetId
                    )
                ) IS NULL
            ) ";
            sqlParams["@Tags"] = parameters.AllTagIds;
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
            sql += $"\n ORDER BY Created {(parameters.SortAscendingByDate ? "ASC" : "DESC")} ";
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


    public async Task<IEnumerable<SimplePostObject.Raw>> GetByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_SimplePostTags termSetsData,
                ClientDataAccess_SimplePosts.GetByCriteria_Params parameters ) {
        if( parameters.AllTagIds.Any(id => id == 0) ) {
            throw new ArgumentException( "Some TermIds are not valid (must be non-zero)." );
        }

        if( parameters.PostsPerPage == 0 ) {
            return Enumerable.Empty<SimplePostObject.Raw>();
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql( parameters, false );

// this.Logger.LogInformation( "Executing SQL: {Sql} with params {Params}", sql, sqlParams );
        IEnumerable<SimplePostObject.Raw> postsRaw = await dbCon.QueryAsync<SimplePostObject.Raw>(
            sql, new DynamicParameters( sqlParams )
        );

        return postsRaw;

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
        if( parameters.AllTagIds.Any(id => id == 0) ) {
            throw new ArgumentException( "Some TermIds are not valid (must be non-zero)." );
        }

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


	public async Task<SimplePostId> Create_Async(
                IDbConnection dbCon,
                ServerDataAccess_ServerData serverData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_SimplePostTags termSetsData,
                ServerDataAccess_UserTermsHistory termHistoryData,
                SimpleUserId simpleUserId,
                ClientDataAccess_SimplePosts.Create_Params parameters,
                bool skipHistory ) {
        if( simpleUserId == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( parameters.TermIds.Any(id => id == 0) ) {
            throw new ArgumentException( "Some TermIds are not valid (must be non-zero)." );
        }

        DateTime now = DateTime.UtcNow;

        long newPostId = await dbCon.ExecuteScalarAsync<long>(   //ExecuteAsync + ExecuteScalarAsync?
            $@"INSERT INTO {TableName} (Created, Modified, Body) 
                VALUES (@Created, @Created, @Body);
            SELECT LAST_INSERT_ID();",
            new {
                Created = now,
                Body = new DbString { Value = parameters.Body, IsAnsi = true }
            }
        );
        
        await termSetsData.CreateForSimplePost_Async( dbCon, (SimplePostId)newPostId, parameters.TermIds );

        //

        IEnumerable<TermId> termIds = parameters.TermIds;
        // ServerDataObject.Raw? serverDataObj = await serverData.Get_Async( dbCon );
        // if( serverDataObj is null ) {
        //     throw new Exception( "Server application data not found." );
        // }

        // IEnumerable<TermId> termIds = parameters.TermIds.Append( serverDataObj.UserConceptTermId );

        //

        if( !skipHistory ) {
            foreach( TermId termId in termIds ) {
                await termHistoryData.AddTerm_Async(
                    dbCon,
                    simpleUserId,
                    new ClientDataAccess_UserTermsHistory.AddTermsForCurrentUser_Params {
                        TermId = termId
                    }
                );
            }
        }

        //

        return (SimplePostId)newPostId;
    }
}
