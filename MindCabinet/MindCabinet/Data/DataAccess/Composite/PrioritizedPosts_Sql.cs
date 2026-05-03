using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Data;
using static MindCabinet.Data.DataAccess.ServerDataAccess_SimplePosts;


namespace MindCabinet.Data.DataAccess.Composite;


public partial class ServerDataAccess_PrioritizedPosts : IServerDataAccess {
    private (string sql, IDictionary<string, object> sqlParams) GetByCriteriaSql(
                PostsContextObject.Raw postsContext,
                string? bodyPattern,
                bool sortAscendingByDate,
                int postsPerPage,
                int pageNumber,
                TermId[] additionalRequiredTagIds,
                bool countOnly ) {
        string sqlColumns = "MyPosts.Id, MyPosts.Created, MyPosts.Modified, MyPosts.SimpleUserId, MyPosts.Body";
        
        string sql = $"SELECT {sqlColumns} FROM {ServerDataAccess_SimplePosts.TableName} AS MyPosts ";
        var sqlParams = new Dictionary<string, object>();

        //

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

        //

        IEnumerable<TermId> allTagIds = additionalRequiredTagIds.Concat(
            postsContext.GetRequiredEntries()
                .Select( e => e.TermId )
                .Where( id => !additionalRequiredTagIds.Contains(id) )
        );
        IEnumerable<TermId> anyTagIds = postsContext.GetOptionalEntries()
            .Select( e => e.TermId );

        if( allTagIds.Count() > 0 ) {
            // sql += hasWhere ? "AND" : "WHERE";
            // sql += $@" (
            //     (
            //         (SELECT (@AllTags)) EXCEPT (
            //             SELECT MyAllTerms.Id FROM {ServerDataAccess_Terms.TableName} AS MyAllTerms
            //             INNER JOIN {ServerDataAccess_TermSets.TableName} AS MyAllTermSet ON (MyAllTermSet.TermId = MyAllTerms.Id)
            //             WHERE MyAllTermSet.SimplePostId = MyPosts.Id
            //         )
            //     ) IS NULL
            // ) ";
            //
            // hasWhere = true;

            sql += $@"
                INNER JOIN {ServerDataAccess_SimplePostTags.TableName} AS MyAllTermSet
                    ON MyAllTermSet.SimplePostId = MyPosts.Id
                WHERE MyAllTermSet.TermId IN (@AllTags)
                GROUP BY MyPosts.Id
                HAVING COUNT(DISTINCT MyAllTermSet.TermId) = @AllTagsCount ";
            sqlParams["@AllTags"] = allTagIds;
            sqlParams["@AllTagsCount"] = allTagIds.Count();
        }
        
        if( anyTagIds.Count() > 0 ) {
            sql += hasWhere ? "AND" : "WHERE";
            sql += $@" (
                (
                    (SELECT (@AnyTags)) INTERSECT (
                        SELECT MyAnyTerms.Id FROM {ServerDataAccess_Terms.TableName} AS MyAnyTerms
                        INNER JOIN {ServerDataAccess_SimplePostTags.TableName} AS MyAnyTermSet ON (MyAnyTermSet.TermId = MyAnyTerms.Id)
                        WHERE MyAnyTermSet.SimplePostId = MyPosts.Id
                    )
                ) IS NOT NULL
            ) ";
            sqlParams["@AnyTags"] = anyTagIds;

            hasWhere = true;
        }

        //

        // IDictionary<long, double> anyAndAllTagIds = postsContext.Entries
        //     .Select( e => new KeyValuePair<long, double>(e.Term.Id, e.Priority) )
        //     .ToDictionary();
        //
        // if( anyAndAllTagIds.Count() > 0 ) {
        //     sql += $@" INNER JOIN ...";
        //     sqlParams["@AnyAndAllTags"] = anyAndAllTagIds.Keys;
        //
        //     hasWhere = true;
        // }

        //

        if( !countOnly ) {
            sql += $"\n ORDER BY Created {(sortAscendingByDate ? "ASC" : "DESC")}";
        }

        //

        if( postsPerPage > 0 ) {
            sql += $"\n LIMIT @Offset, @Quantity";
            sqlParams["@Offset"] = pageNumber * postsPerPage;
            sqlParams["@Quantity"] = postsPerPage;
        }

        if( countOnly ) {
            sql = $"SELECT COUNT(*) FROM ({sql}) AS CountQuery";
        }

        sql += ";";
        return (sql, sqlParams);
    }
}
