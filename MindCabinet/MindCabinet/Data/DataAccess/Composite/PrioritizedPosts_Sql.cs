using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Client.Services.DbAccess.Joined;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using System.Data;
using MindCabinet.Utility;
using System.Text.Json;


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
        SimpleSqlSelectBuilder sqlBuilder = new SimpleSqlSelectBuilder(
            tableName: $"{ServerDataAccess_SimplePosts.TableName} AS MyPosts",
            columnNames: ServerDataAccess_SimplePosts.TableColumns.Keys
                .Select( col => $"MyPosts.{col}" )
        );
        sqlBuilder.WrapWithCount = countOnly;

        var sqlParams = new Dictionary<string, object>();

        //

        if( !string.IsNullOrEmpty(bodyPattern) ) {
            // // sql += "WHERE MyPosts.Body LIKE REPLACE(REPLACE(REPLACE(@Body, '[', '[[]'), '_', '[_]'), '%', '[%]')";
            // sql += "\nWHERE MyPosts.Body LIKE @Body ESCAPE '\\\\' ";
            sqlBuilder.AddWhereClause( $"MyPosts.{ServerDataAccess_SimplePosts.TableColumn_Body} LIKE @Body ESCAPE '\\\\'" );

            string body = bodyPattern.Replace( "%", "\\%" );
            body = body.Replace( "_", "\\_" );
            //body = body.Replace( "[", "\\[" );

            sqlParams["@Body"] = new DbString { Value = $"%{body}%", IsAnsi = true };
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
            //             INNER JOIN {ServerDataAccess_TermSets.TableName} AS MyAllPostTags ON (MyAllPostTags.TermId = MyAllTerms.Id)
            //             WHERE MyAllPostTags.SimplePostId = MyPosts.Id
            //         )
            //     ) IS NULL
            // ) ";
            //
            // hasWhere = true;

            sqlBuilder.JoinClause = $"INNER JOIN {ServerDataAccess_SimplePostTags.TableName} AS MyAllPostTags"
                + $"\n    ON MyAllPostTags.{ServerDataAccess_SimplePostTags.TableColumn_SimplePostId} = MyPosts.{ServerDataAccess_SimplePosts.TableColumn_Id}";
            sqlBuilder.AddWhereClause( $"MyAllPostTags.{ServerDataAccess_SimplePostTags.TableColumn_TermId} IN @AllTags" );
            sqlBuilder.GroupByClause = $"MyPosts.{ServerDataAccess_SimplePosts.TableColumn_Id}";
            sqlBuilder.HavingClause = $"COUNT(DISTINCT MyAllPostTags.{ServerDataAccess_SimplePostTags.TableColumn_TermId}) = @AllTagsCount";

            sqlParams["@AllTags"] = allTagIds;
            sqlParams["@AllTagsCount"] = allTagIds.Count();
        }
        
        if( anyTagIds.Count() > 0 ) {
            sqlBuilder.AddWhereClause( $@" (
                (
                    (SELECT (@AnyTags)) INTERSECT (
                        SELECT MyAnyTerms.{ServerDataAccess_Terms.TableColumn_Id} FROM {ServerDataAccess_Terms.TableName} AS MyAnyTerms
                        INNER JOIN {ServerDataAccess_SimplePostTags.TableName}
                            AS MyAnyPostTags
                            ON (MyAnyPostTags.{ServerDataAccess_SimplePostTags.TableColumn_TermId} = MyAnyTerms.{ServerDataAccess_Terms.TableColumn_Id})
                        WHERE MyAnyPostTags.{ServerDataAccess_SimplePostTags.TableColumn_SimplePostId} = MyPosts.{ServerDataAccess_SimplePosts.TableColumn_Id}
                    )
                ) IS NOT NULL
            )" );

            sqlParams["@AnyTags"] = anyTagIds;
        }

        //

        if( !countOnly ) {
            sqlBuilder.OrderByClause = $"MyPosts.{ServerDataAccess_SimplePosts.TableColumn_Created} {(sortAscendingByDate ? "ASC" : "DESC")}";
        }

        //

        if( postsPerPage > 0 ) {
            sqlBuilder.LimitClause = "@Offset, @Quantity";

            sqlParams["@Offset"] = pageNumber * postsPerPage;
            sqlParams["@Quantity"] = postsPerPage;
        }

// this.Logger.LogInformation( "Generated SQL: {Sql} with params {Params}",
//     sqlBuilder.Build(),
//     JsonSerializer.Serialize(sqlParams)
// );
        return (sqlBuilder.Build(), sqlParams);
    }
}
