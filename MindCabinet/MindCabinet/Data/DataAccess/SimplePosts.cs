using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.DataObjects;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;
using MindCabinet.Utility;
using System.Data;
using System.Text.Json;


namespace MindCabinet.Data.DataAccess;



public partial class ServerDataAccess_SimplePosts(
                ILogger<ServerDataAccess_SimplePosts> logger,
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    private static readonly SimpleCache<SimplePostId, SimplePostObject.Raw?> Cache_ById = new( refreshExpiryOnGet: true );



    private readonly ILogger<ServerDataAccess_SimplePosts> Logger = logger;
    
    private readonly StaticServerSettings ServerSettings = serverSettings;



    public async Task<SimplePostObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                SimplePostId id ) {
        if( id == 0 ) {
            throw new ArgumentException( "SimplePostId is not valid (must be non-zero)." );
        }

        //

        if( ServerDataAccess_SimplePosts.Cache_ById.TryGet( id, out var cached ) ) {
            return cached;
        }

        //

        SimplePostObject.Raw? postRaw = await dbCon.QuerySingleOrDefaultAsync<SimplePostObject.Raw>(
            $"SELECT * FROM {TableName} AS MyPosts WHERE {TableColumn_Id} = @Id",
            new { Id = (long)id }
        );

        //

        ServerDataAccess_SimplePosts.Cache_ById.Set(
            key: id,
            value: postRaw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return postRaw;
    }


    private (string sql, IDictionary<string, object> sqlParams) GetByCriteriaSql(
                ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Params parameters,
                SimpleUserId? author,
                bool countOnly ) {
        var sqlParams = new Dictionary<string, object>();
        var sqlBuilder = new SimpleSqlSelectBuilder(
            tableName: TableName,
            columnNames: countOnly ? TableColumns.Keys : ["COUNT(*)"]
        );

        
        if( author is not null ) {
            sqlBuilder.AddWhereClause( $"MyPosts.{TableColumn_Author} = @Author" );

            sqlParams["@Author"] = author;
        }
        
        if( !string.IsNullOrEmpty(parameters.BodyPattern) ) {
            // sql += "WHERE MyPosts.{TableColumn_Body} LIKE REPLACE(REPLACE(REPLACE(@Body, '[', '[[]'), '_', '[_]'), '%', '[%]')";
            sqlBuilder.AddWhereClause( $"MyPosts.{TableColumn_Body} LIKE @Body ESCAPE '\\\\' " );

            string body = parameters.BodyPattern.Replace( "%", "\\%" );
            body = body.Replace( "_", "\\_" );
            //body = body.Replace( "[", "\\[" );

            sqlParams["@Body"] = new DbString { Value = $"%{body}%", IsAnsi = true };
        }

        if( parameters.AllTagIds.Length > 0 ) {
            sqlBuilder.AddWhereClause( $@" (
                (
                    (SELECT (@Tags)) EXCEPT (
                        SELECT MyTerms.{ServerDataAccess_Terms.TableColumn_Id} FROM {ServerDataAccess_Terms.TableName} AS MyTerms
                        INNER JOIN {ServerDataAccess_SimplePostTags.TableName} AS MyPostTags
                            ON (MyPostTags.{ServerDataAccess_SimplePostTags.TableColumn_TermId} = MyTerms.{ServerDataAccess_Terms.TableColumn_Id})
                        WHERE MyPostTags.{ServerDataAccess_SimplePostTags.TableColumn_SimplePostId} = MyPosts.{ServerDataAccess_SimplePosts.TableColumn_Id}
                    )
                ) IS NULL
            ) " );

            sqlParams["@Tags"] = parameters.AllTagIds;
        }
        
        if( !countOnly ) {
            sqlBuilder.OrderByClause = $" ORDER BY Created {(parameters.SortAscendingByDate ? "ASC" : "DESC")} ";
        }

        if( parameters.PostsPerPage > 0 ) {
            sqlBuilder.LimitClause = $" LIMIT @Offset, @Quantity";
            // FETCH NEXT @Quantity ROWS ONLY;";
            // LIMIT @Offset ROWS OFFSET @Quantity ROWS ONLY;";
            sqlParams["@Offset"] = parameters.PageNumber * parameters.PostsPerPage;
            sqlParams["@Quantity"] = parameters.PostsPerPage;
        }

        return (sqlBuilder.Build(), sqlParams);
    }


    public async Task<IEnumerable<SimplePostObject.Raw>> GetByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_Terms termsDataSrc,
                ServerDataAccess_SimplePostTags termSetsDataSrc,
                ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Params parameters,
                SimpleUserId? author ) {
        if( parameters.AllTagIds.Any(id => id == 0) ) {
            throw new ArgumentException( "Some TermIds are not valid (must be non-zero)." );
        }

        if( parameters.PostsPerPage == 0 ) {
            return Enumerable.Empty<SimplePostObject.Raw>();
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql(
            parameters: parameters,
            author: author,
            countOnly: false
        );

// this.Logger.LogInformation( "Executing SQL: {Sql} with params {Params}", sql, sqlParams );
        IEnumerable<SimplePostObject.Raw> postsRaw = await dbCon.QueryAsync<SimplePostObject.Raw>(
            sql, new DynamicParameters( sqlParams )
        );

        //

        foreach( SimplePostObject.Raw rawPost in postsRaw ) {
            ServerDataAccess_SimplePosts.Cache_ById.Set(
                key: rawPost.Id,
                value: rawPost,
                expiry: this.ServerSettings.CacheExpirationDuration
            );
        }

        //

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
                ClientDataAccess_SimplePosts.IAPI.GetByCriteria_Params parameters,
                SimpleUserId? author ) {
        if( parameters.AllTagIds.Any(id => id == 0) ) {
            throw new ArgumentException( "Some TermIds are not valid (must be non-zero)." );
        }

        if( parameters.PostsPerPage == 0 ) {
            return 0;
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql( parameters, author, true );

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


	public async Task<SimplePostObject.Raw> Create_Async(
                IDbConnection dbCon,
                ServerDataAccess_ServerData serverData,
                ServerDataAccess_UserAppData userData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_SimplePostTags termSetsData,
                ServerDataAccess_UserTermsHistory termHistoryData,
                SimpleUserId author,
                ClientDataAccess_SimplePosts.IAPI.Create_Params parameters,
                bool skipHistory ) {
        if( author == 0 ) {
            throw new ArgumentException( "SimpleUserId is not valid (must be non-zero)." );
        }
        if( parameters.TermIds.Any(id => id == 0) ) {
            throw new ArgumentException( "Some TermIds are not valid (must be non-zero)." );
        }

        //

        DateTime now = DateTime.UtcNow;

        long newPostId = await dbCon.ExecuteScalarAsync<long>(   //ExecuteAsync + ExecuteScalarAsync?
            $@"INSERT INTO {TableName}
                    ({TableColumn_Created}, {TableColumn_Modified}, {TableColumn_Author}, {TableColumn_Body}) 
                VALUES
                    (@{TableColumn_Created}, @{TableColumn_Modified}, @{TableColumn_Author}, @{TableColumn_Body});
            SELECT LAST_INSERT_ID();",
            new {
                Created = now,
                Modified = now,
                SimpleUserId = (long)author,
                Body = new DbString { Value = parameters.Body, IsAnsi = true }
            }
        );
        
        await termSetsData.CreateForSimplePost_Async(
            dbCon: dbCon,
            termsDataSrc: termsData,
            id: (SimplePostId)newPostId,
            termIds: parameters.TermIds
        );

        //

        SimplePostObject.Raw raw = SimplePostObject.CreateRaw(
            id: (SimplePostId)newPostId,
            created: now,
            modified: now,
            author: author,
            body: parameters.Body,
            tagsTermIdSet: parameters.TermIds
        );

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
                    author,
                    new ClientDataAccess_UserTermsHistory.IAPI.AddHistTermsForCurrentUser_Params {
                        TermId = termId
                    }
                );
            }
        }

        //

        ServerDataAccess_SimplePosts.Cache_ById.Set(
            key: (SimplePostId)newPostId,
            value: raw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return raw;
    }
}
