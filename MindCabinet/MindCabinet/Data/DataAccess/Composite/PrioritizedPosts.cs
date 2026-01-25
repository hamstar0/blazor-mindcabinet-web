using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.UserContext;
using System.Data;
using static MindCabinet.Data.DataAccess.ServerDataAccess_SimplePosts;


namespace MindCabinet.Data.DataAccess.Composite;


public partial class ServerDataAccess_PrioritizedPosts : IServerDataAccess {
    public string TableName => ServerDataAccess_SimplePosts.TableName;
    

    
    private (string sql, IDictionary<string, object> sqlParams) GetByCriteriaSql(
                UserContextObject userContext,
                string? bodyPattern,
                bool sortAscendingByDate,
                int postsPerPage,
                int pageNumber,
                long[] additionalTagIds,
                bool countOnly ) {
        string sqlColumns = countOnly
            ? "COUNT(*)"
            : "*";  //"MyPosts.Id, MyPosts.Created, MyPosts.Modified, MyPosts.SimpleUserId, MyPosts.Body, MyPosts.TermSetId";
        string sql = $"SELECT {sqlColumns} FROM {TableName} AS MyPosts ";
        var sqlParams = new Dictionary<string, object>();

        bool hasWhere = false;

        if( !string.IsNullOrEmpty(bodyPattern) ) {
            string body = bodyPattern.Replace( "%", "\\%" );
            body = body.Replace( "_", "\\_" );
            //body = body.Replace( "[", "\\[" );

            // sql += "WHERE MyPosts.Body LIKE REPLACE(REPLACE(REPLACE(@Body, '[', '[[]'), '_', '[_]'), '%', '[%]')";
            sql += "\nWHERE MyPosts.Body LIKE @Body ESCAPE '\\\\' ";
            sqlParams["@Body"] = new DbString { Value = $"%{body}%", IsAnsi = true };
            hasWhere = true;
        }

        long[] allTagIds = additionalTagIds;
        allTagIds = allTagIds.Concat(
            userContext.GetRequiredEntries()
                .Select( e => e.Term.Id )
                .Where( id => !allTagIds.Contains(id) )
            ).ToArray();
        long[] anyTagIds = userContext.GetOptionalEntries()
            .Select( e => e.Term.Id )
            .ToArray();

        if( allTagIds.Length > 0 ) {
            sql += hasWhere ? "AND" : "WHERE";
            sql += $@" (
                (
                    (SELECT (@AllTags)) EXCEPT (
                        SELECT MyAllTerms.Id FROM {ServerDataAccess_Terms.TableName} AS MyAllTerms
                        INNER JOIN {ServerDataAccess_Terms_Sets.TableName} AS MyAllTermSet ON (MyAllTermSet.TermId = MyAllTerms.Id)
                        WHERE MyAllTermSet.SetId = MyPosts.TermSetId
                    )
                ) IS NULL
            ) ";
            sqlParams["@AllTags"] = allTagIds;
            
            hasWhere = true;
        }
        
        if( anyTagIds.Length > 0 ) {
            sql += hasWhere ? "AND" : "WHERE";
            sql += $@" (
                (
                    (SELECT (@AnyTags)) INTERSECT (
                        SELECT MyAnyTerms.Id FROM {ServerDataAccess_Terms.TableName} AS MyAnyTerms
                        INNER JOIN {ServerDataAccess_Terms_Sets.TableName} AS MyAnyTermSet ON (MyAnyTermSet.TermId = MyAnyTerms.Id)
                        WHERE MyAnyTermSet.SetId = MyPosts.TermSetId
                    )
                ) IS NOT NULL
            ) ";
            sqlParams["@AnyTags"] = anyTagIds;

            hasWhere = true;
        }

        if( !countOnly ) {
            sql += $"\n ORDER BY Created {(sortAscendingByDate ? "ASC" : "DESC")}";
        }

        if( postsPerPage > 0 ) {
            sql += $"\n LIMIT @Offset, @Quantity";
            sqlParams["@Offset"] = pageNumber * postsPerPage;
            sqlParams["@Quantity"] = postsPerPage;
        }

        sql += ";";
        return (sql, sqlParams);
    }


    public async Task<IEnumerable<SimplePostObject.DatabaseEntry>> GetByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_UserContext userContextData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_Terms_Sets termSetsData,
                ClientDataAccess_PrioritizedPosts.GetByCriteria_Params parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return [];
        }

        UserContextObject? usrCtx = await userContextData.GetById_Async( dbCon, termsData, termSetsData, parameters.UserContextId );
        if( usrCtx is null ) {
            return [];
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql(
                userContext: usrCtx,
                bodyPattern: parameters.BodyPattern,
                sortAscendingByDate: parameters.SortAscendingByDate,
                postsPerPage: parameters.PostsPerPage,
                pageNumber: parameters.PageNumber,
                additionalTagIds: parameters.AdditionalTagIds,
                countOnly: false
        );

        // this.Logger.LogInformation( "Executing SQL: {Sql} with params {Params}", sql, sqlParams );
        IEnumerable<SimplePostObject.DatabaseEntry> posts = await dbCon.QueryAsync<SimplePostObject.DatabaseEntry>(
            sql, new DynamicParameters( sqlParams )
        );

        return posts;
	}


    public async Task<int> GetCountByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_UserContext userContextData,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_Terms_Sets termSetsData,
                ClientDataAccess_PrioritizedPosts.GetByCriteria_Params parameters ) {
        if( parameters.PostsPerPage == 0 ) {
            return 0;
        }

        UserContextObject? usrCtx = await userContextData.GetById_Async( dbCon, termsData, termSetsData, parameters.UserContextId );
        if( usrCtx is null ) {
            return 0;
        }

        (string sql, IDictionary<string, object> sqlParams) = this.GetByCriteriaSql(
                userContext: usrCtx,
                bodyPattern: parameters.BodyPattern,
                sortAscendingByDate: parameters.SortAscendingByDate,
                postsPerPage: parameters.PostsPerPage,
                pageNumber: parameters.PageNumber,
                additionalTagIds: parameters.AdditionalTagIds,
                countOnly: true
        );

        return await dbCon.QuerySingleAsync<int>( sql, new DynamicParameters(sqlParams) );
    }
}
