using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsQuery;
using MindCabinet.Shared.Utility;
using System.Data;
using MindCabinet.Services;
using System.Text.Json;
using MindCabinet.Utility;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_PostsQuery(
                ILogger<ServerDataAccess_PostsQuery> logger,
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    private static readonly SimpleCache<PostsQueryId, PostsQueryObject.Raw?> Cache_ById = new( refreshExpiryOnGet: true );



    private readonly ILogger<ServerDataAccess_PostsQuery> Logger = logger;

    private readonly StaticServerSettings ServerSettings = serverSettings;



    public async Task<PostsQueryObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsQueryTermEntry postsQueryTermEntryDataSrc,
                PostsQueryId postsQueryId,
                bool alsoGetEntries ) {
        if( postsQueryId == 0 ) {
            throw new ArgumentException( "PostsQueryId is not valid (must be non-zero)." );
        }

        //

        if( Cache_ById.TryGet(postsQueryId, out var cached) ) {
            return cached;
        }

        //

        var raw = await dbCon.QuerySingleOrDefaultAsync<PostsQueryObject.Raw>(
            $"SELECT * FROM {TableName} WHERE {TableColumn_Id} = @Id",
            new { Id = (long)postsQueryId }
        );
        if( raw is null ) {
            return null;
        }

        if( alsoGetEntries ) {
            raw.Entries = (await postsQueryTermEntryDataSrc.GetByPostsQueryId_Async(
                dbCon: dbCon,
                postsQueryId: raw.Id
            )).ToArray();
        }

        //

        Cache_ById.Set(
            key: raw.Id,
            value: raw,
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        return raw;
    }


    public async Task<IEnumerable<PostsQueryObject.Raw>> GetByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsQueryTermEntry postsQueryTermEntryDataSrc,
                ClientDataAccess_PostsQuery.IAPI.GetByCriteria_Params parameters,
                SimpleUserId? owner,
                bool alsoGetEntries ) {
        if( parameters.Ids.Any(id => id == 0) ) {
            throw new ArgumentException( "Some PostsQueryIds are not valid (must be non-zero)." );
        }
        if( parameters.TagTermIds.Any(id => id == 0) ) {
            throw new ArgumentException( "Some TagTermIds are not valid (must be non-zero)." );
        }

        var sqlBuilder = new SimpleSqlSelectBuilder(
            tableName: $"{TableName} AS MyQuery",
            columnNames: TableColumns.Select( col => $"MyQuery.{col}" )
        );
        var sqlParams1 = new Dictionary<string, object>();

        if( owner is not null ) {
            sqlBuilder.AddWhereClause( $"MyQuery.{TableColumn_Owner} = @Owner" );
            sqlParams1.Add( "@Owner", (long)owner.Value );
        }

        if( parameters.Ids.Length >= 2 ) {
            sqlBuilder.AddWhereClause( $"MyQuery.{TableColumn_Id} IN @Ids" );
            sqlParams1.Add( "@Ids", parameters.Ids );
        } else if( parameters.Ids.Length == 1 ) {
            sqlBuilder.AddWhereClause( $"MyQuery.{TableColumn_Id} = @Id" );
            sqlParams1.Add( "@Id", parameters.Ids[0] );
        }

        if( parameters.TagTermIds.Any() ) {
            sqlBuilder.JoinClause = $"INNER JOIN {ServerDataAccess_PostsQueryTermEntry.TableName} AS MyQueryTags";
            sqlBuilder.JoinClause += $"\n ON MyQuery.{TableColumn_Id} = MyQueryTags.{ServerDataAccess_PostsQueryTermEntry.TableColumn_PostsQueryId}";

            sqlBuilder.AddWhereClause(
                $"MyQueryTags.{ServerDataAccess_PostsQueryTermEntry.TableColumn_TermId} IN @TagTermIds"
            );
            sqlParams1.Add( "@TagTermIds", parameters.TagTermIds );
        }

        if( !string.IsNullOrEmpty(parameters.NameContains) ) {
            sqlBuilder.AddWhereClause(
                $"MyQuery.{TableColumn_Name} LIKE @NameContains ESCAPE '\\\\'"
            );

            string nameContains = parameters.NameContains.Replace( "%", "\\%" );
            nameContains = nameContains.Replace( "_", "\\_" );
            //nameContains = nameContains.Replace( "[", "\\[" );

            sqlParams1["@NameContains"] = new DbString { Value = $"%{nameContains}%", IsAnsi = true };
        }

//this.Logger.LogInformation( "SQL: "+sqlBuilder.Build()+" PARAMS: "+JsonSerializer.Serialize(sqlParams1) );
        IEnumerable<PostsQueryObject.Raw> queries = await dbCon.QueryAsync<PostsQueryObject.Raw>(
            sql: sqlBuilder.Build(),
            param: new DynamicParameters(sqlParams1)
        );

        if( alsoGetEntries ) {
            foreach( PostsQueryObject.Raw rawQuery in queries ) {
                rawQuery.Entries = (await postsQueryTermEntryDataSrc.GetByPostsQueryId_Async(
                    dbCon: dbCon,
                    postsQueryId: rawQuery.Id
                )).ToArray();

                //

                Cache_ById.Set(
                    key: rawQuery.Id,
                    value: rawQuery,
                    expiry: this.ServerSettings.CacheExpirationDuration
                );
            }
        }

        return queries;
    }


    public async Task<ClientDataAccess_PostsQuery.IAPI.CreateOrUpdate_Return> Create_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsQueryTermEntry postsQueryTermEntryDataSrc,
                PostsQueryObject.Prototype parameters,
                SimpleUserId owner ) {
        if( !PostsQueryObject.ValidateName(parameters.Name ?? "") ) {
            throw new ArgumentException( "PostsQuery Name is not valid." );
        }
        if( !PostsQueryObject.Prototype.ValidateEntries(parameters.Entries, false) ) {
            throw new ArgumentException(
                "PostsQuery Entries are not valid: "
                + string.Join(", ", parameters.Entries
                    .Where( e => !e.IsValid(false) )
                    .Select( e => "term:"+e.TermId )
                )
            );
        }
        if( !PostsQueryObject.ValidateOwner(owner) ) {
            throw new ArgumentException( "PostsQueryObject.Prototype owner is not valid." );
        }

        long postsQueryIdL = await dbCon.ExecuteScalarAsync<long>(
            $@"INSERT INTO {TableName}
                ({TableColumn_Name}, {TableColumn_Description}, {TableColumn_Owner})
                VALUES (@Name, @Description, @Owner);
            SELECT LAST_INSERT_ID();",
            new {
                Name = parameters.Name,
                Description = parameters.Description,
                Owner = owner
            }
        );
        PostsQueryId postsQueryId = (PostsQueryId)postsQueryIdL;

        //

        PostsQueryTermEntryObject.Raw[] entries = parameters.Entries
            .Select( e => e.ToRaw(false, true) )
            .ToArray();

        foreach( PostsQueryTermEntryObject.Raw entry in entries ) {
            await postsQueryTermEntryDataSrc.Create_Async(
                dbCon: dbCon,
                postsQueryId: postsQueryId,
                parameter: entry
            );
        }

        //

        Cache_ById.Set(
            key: postsQueryId,
            value: PostsQueryObject.CreateRaw(
                id: postsQueryId,
                name: parameters.Name!,
                description: parameters.Description,
                owner: owner,
                entries: entries
            ),
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return new ClientDataAccess_PostsQuery.IAPI.CreateOrUpdate_Return { Id = postsQueryId };
    }


    public async Task<ClientDataAccess_PostsQuery.IAPI.CreateOrUpdate_Return> Update_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsQueryTermEntry postsQueryTermEntryDataSrc,
                PostsQueryObject.Prototype parameters ) {
        if( !PostsQueryObject.ValidateId(parameters.Id ?? 0) ) {
            throw new ArgumentException( "PostsQueryObject.Prototype Id is not valid." );
        }
        if( !PostsQueryObject.ValidateName(parameters.Name ?? "") ) {
            throw new ArgumentException( "PostsQueryObject.Prototype Name is not valid." );
        }
        if( !PostsQueryObject.ValidateOwner(parameters.Owner ?? 0) ) {
            throw new ArgumentException( "PostsQueryObject.Prototype Owner is not valid." );
        }

        await dbCon.ExecuteAsync(
            $@"UPDATE {TableName}
                SET {TableColumn_Name} = @Name, {TableColumn_Description} = @Description
                WHERE {TableColumn_Id} = @Id;",
            new {
                Name = parameters.Name,
                Description = parameters.Description,
                Id = parameters.Id
            }
        );
        
        await postsQueryTermEntryDataSrc.DeleteByPostsQueryId_Async(
            dbCon: dbCon,
            postsQueryId: parameters.Id!.Value
        );

        PostsQueryTermEntryObject.Raw[] entries = parameters.Entries
            .Select( e => e.ToRaw(false, true) )
            .ToArray();

        foreach( PostsQueryTermEntryObject.Raw entry in entries ) {
            await postsQueryTermEntryDataSrc.Create_Async(
                dbCon: dbCon,
                postsQueryId: parameters.Id!.Value,
                parameter: entry
            );
        }

        //

        Cache_ById.Set(
            key: parameters.Id.Value,
            value: PostsQueryObject.CreateRaw(
                id: parameters.Id.Value,
                name: parameters.Name!,
                description: parameters.Description,
                owner: parameters.Owner!.Value,
                entries: entries
            ),
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        return new ClientDataAccess_PostsQuery.IAPI.CreateOrUpdate_Return {
            Id = parameters.Id.Value
        };
    }
}
