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

        IEnumerable<long> allTagIds = additionalTagIds.Concat(
            userContext.GetRequiredEntries()
                .Select( e => e.Term.Id )
                .Where( id => !additionalTagIds.Contains(id) )
        );
        IEnumerable<long> anyTagIds = userContext.GetOptionalEntries()
            .Select( e => e.Term.Id );

        if( allTagIds.Count() > 0 ) {
            sql += hasWhere ? "AND" : "WHERE";
            sql += $@" (
                (
                    (SELECT (@AllTags)) EXCEPT (
                        SELECT MyAllTerms.Id FROM {ServerDataAccess_Terms.TableName} AS MyAllTerms
                        INNER JOIN {ServerDataAccess_TermSets.TableName} AS MyAllTermSet ON (MyAllTermSet.TermId = MyAllTerms.Id)
                        WHERE MyAllTermSet.SetId = MyPosts.TermSetId
                    )
                ) IS NULL
            ) ";
            sqlParams["@AllTags"] = allTagIds;
            
            hasWhere = true;
        }
        
        if( anyTagIds.Count() > 0 ) {
            sql += hasWhere ? "AND" : "WHERE";
            sql += $@" (
                (
                    (SELECT (@AnyTags)) INTERSECT (
                        SELECT MyAnyTerms.Id FROM {ServerDataAccess_Terms.TableName} AS MyAnyTerms
                        INNER JOIN {ServerDataAccess_TermSets.TableName} AS MyAnyTermSet ON (MyAnyTermSet.TermId = MyAnyTerms.Id)
                        WHERE MyAnyTermSet.SetId = MyPosts.TermSetId
                    )
                ) IS NOT NULL
            ) ";
            sqlParams["@AnyTags"] = anyTagIds;

            hasWhere = true;
        }

        //

        // IDictionary<long, double> anyAndAllTagIds = userContext.Entries
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

        sql += ";";
        return (sql, sqlParams);
    }
}
