using Dapper;
using Microsoft.Data.SqlClient;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.DataObjects.PostsContext;
using MindCabinet.Shared.Utility;
using System.Data;
using MindCabinet.Services;
using System.Text.Json;
using MindCabinet.Utility;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_PostsContexts(
                ILogger<ServerDataAccess_PostsContexts> logger,
                StaticServerSettings serverSettings
            ) : IServerDataAccess {
    private static readonly SimpleCache<PostsContextId, PostsContextObject.Raw?> Cache_ById = new( refreshExpiryOnGet: true );



    private readonly ILogger<ServerDataAccess_PostsContexts> Logger = logger;

    private readonly StaticServerSettings ServerSettings = serverSettings;



    public async Task<PostsContextObject.Raw?> GetById_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                PostsContextId postsContextId,
                bool alsoGetEntries ) {
        if( postsContextId == 0 ) {
            throw new ArgumentException( "PostsContextId is not valid (must be non-zero)." );
        }

        //

        if( Cache_ById.TryGet(postsContextId, out var cached) ) {
            return cached;
        }

        //

        var raw = await dbCon.QuerySingleOrDefaultAsync<PostsContextObject.Raw>(
            $"SELECT * FROM {TableName} WHERE {TableColumn_Id} = @Id",
            new { Id = (long)postsContextId }
        );
        if( raw is null ) {
            return null;
        }

        if( alsoGetEntries ) {
            raw.Entries = (await postsContextTermEntryDataSrc.GetByPostsContextId_Async(
                dbCon: dbCon,
                postsContextId: raw.Id
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


    public async Task<IEnumerable<PostsContextObject.Raw>> GetByCriteria_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                ClientDataAccess_PostsContext.IAPI.GetByCriteria_Params parameters,
                SimpleUserId? owner,
                bool alsoGetEntries ) {
        if( parameters.Ids.Any(id => id == 0) ) {
            throw new ArgumentException( "Some PostsContextIds are not valid (must be non-zero)." );
        }
        if( parameters.TagTermIds.Any(id => id == 0) ) {
            throw new ArgumentException( "Some TagTermIds are not valid (must be non-zero)." );
        }

        var sqlBuilder = new SimpleSqlSelectBuilder(
            tableName: $"{TableName} AS MyContext",
            columnNames: TableColumns.Select( col => $"MyContext.{col}" )
        );
        var sqlParams1 = new Dictionary<string, object>();

        if( owner is not null ) {
            sqlBuilder.AddWhereClause( $"MyContext.{TableColumn_Owner} = @Owner" );
            sqlParams1.Add( "@Owner", (long)owner.Value );
        }

        if( parameters.Ids.Length >= 2 ) {
            sqlBuilder.AddWhereClause( $"MyContext.{TableColumn_Id} IN @Ids" );
            sqlParams1.Add( "@Ids", parameters.Ids );
        } else if( parameters.Ids.Length == 1 ) {
            sqlBuilder.AddWhereClause( $"MyContext.{TableColumn_Id} = @Id" );
            sqlParams1.Add( "@Id", parameters.Ids[0] );
        }

        if( parameters.TagTermIds.Any() ) {
            sqlBuilder.JoinClause = $"INNER JOIN {ServerDataAccess_PostsContextTermEntry.TableName} AS MyContextTags";
            sqlBuilder.JoinClause += $"\n ON MyContext.{TableColumn_Id} = MyContextTags.{ServerDataAccess_PostsContextTermEntry.TableColumn_PostsContextId}";

            sqlBuilder.AddWhereClause(
                $"MyContextTags.{ServerDataAccess_PostsContextTermEntry.TableColumn_TermId} IN @TagTermIds"
            );
            sqlParams1.Add( "@TagTermIds", parameters.TagTermIds );
        }

        if( !string.IsNullOrEmpty(parameters.NameContains) ) {
            sqlBuilder.AddWhereClause(
                $"MyContext.{TableColumn_Name} LIKE @NameContains ESCAPE '\\\\'"
            );

            string nameContains = parameters.NameContains.Replace( "%", "\\%" );
            nameContains = nameContains.Replace( "_", "\\_" );
            //nameContains = nameContains.Replace( "[", "\\[" );

            sqlParams1["@NameContains"] = new DbString { Value = $"%{nameContains}%", IsAnsi = true };
        }

//this.Logger.LogInformation( "SQL: "+sqlBuilder.Build()+" PARAMS: "+JsonSerializer.Serialize(sqlParams1) );
        IEnumerable<PostsContextObject.Raw> contexts = await dbCon.QueryAsync<PostsContextObject.Raw>(
            sql: sqlBuilder.Build(),
            param: new DynamicParameters(sqlParams1)
        );

        if( alsoGetEntries ) {
            foreach( PostsContextObject.Raw rawCtx in contexts ) {
                rawCtx.Entries = (await postsContextTermEntryDataSrc.GetByPostsContextId_Async(
                    dbCon: dbCon,
                    postsContextId: rawCtx.Id
                )).ToArray();

                //

                Cache_ById.Set(
                    key: rawCtx.Id,
                    value: rawCtx,
                    expiry: this.ServerSettings.CacheExpirationDuration
                );
            }
        }

        return contexts;
    }


    public async Task<ClientDataAccess_PostsContext.IAPI.CreateOrUpdate_Return> Create_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                PostsContextObject.Prototype parameters,
                SimpleUserId owner ) {
        if( !PostsContextObject.ValidateName(parameters.Name ?? "") ) {
            throw new ArgumentException( "PostsContext Name is not valid." );
        }
        if( !PostsContextObject.Prototype.ValidateEntries(parameters.Entries, true) ) {
            throw new ArgumentException(
                "PostsContext Entries are not valid: "
                + string.Join(", ", parameters.Entries
                    .Where( e => !e.IsValid(true) )
                    .Select( e => "term:"+e.TermId )
                )
            );
        }
        if( !PostsContextObject.ValidateOwner(owner) ) {
            throw new ArgumentException( "PostsContextObject.Prototype owner is not valid." );
        }

        long postsContextIdL = await dbCon.ExecuteScalarAsync<long>(
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
        PostsContextId postsContextId = (PostsContextId)postsContextIdL;

        //

        PostsContextTermEntryObject.Raw[] entries = parameters.Entries
            .Select( e => e.ToRaw(false, true) )
            .ToArray();

        foreach( PostsContextTermEntryObject.Raw entry in entries ) {
            await postsContextTermEntryDataSrc.Create_Async(
                dbCon: dbCon,
                postsContextId: postsContextId,
                parameter: entry
            );
        }

        //

        Cache_ById.Set(
            key: postsContextId,
            value: PostsContextObject.CreateRaw(
                id: postsContextId,
                name: parameters.Name!,
                description: parameters.Description,
                owner: owner,
                entries: entries
            ),
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        //

        return new ClientDataAccess_PostsContext.IAPI.CreateOrUpdate_Return { Id = postsContextId };
    }


    public async Task<ClientDataAccess_PostsContext.IAPI.CreateOrUpdate_Return> Update_Async(
                IDbConnection dbCon,
                ServerDataAccess_PostsContextTermEntry postsContextTermEntryDataSrc,
                PostsContextObject.Prototype parameters ) {
        if( !PostsContextObject.ValidateId(parameters.Id ?? 0) ) {
            throw new ArgumentException( "PostsContextObject.Prototype Id is not valid." );
        }
        if( !PostsContextObject.ValidateName(parameters.Name ?? "") ) {
            throw new ArgumentException( "PostsContextObject.Prototype Name is not valid." );
        }
        if( !PostsContextObject.ValidateOwner(parameters.Owner ?? 0) ) {
            throw new ArgumentException( "PostsContextObject.Prototype Owner is not valid." );
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
        
        await postsContextTermEntryDataSrc.DeleteByPostsContextId_Async(
            dbCon: dbCon,
            postsContextId: parameters.Id!.Value
        );

        PostsContextTermEntryObject.Raw[] entries = parameters.Entries
            .Select( e => e.ToRaw(false, true) )
            .ToArray();

        foreach( PostsContextTermEntryObject.Raw entry in entries ) {
            await postsContextTermEntryDataSrc.Create_Async(
                dbCon: dbCon,
                postsContextId: parameters.Id!.Value,
                parameter: entry
            );
        }

        //

        Cache_ById.Set(
            key: parameters.Id.Value,
            value: PostsContextObject.CreateRaw(
                id: parameters.Id.Value,
                name: parameters.Name!,
                description: parameters.Description,
                owner: parameters.Owner!.Value,
                entries: entries
            ),
            expiry: this.ServerSettings.CacheExpirationDuration
        );

        return new ClientDataAccess_PostsContext.IAPI.CreateOrUpdate_Return {
            Id = parameters.Id.Value
        };
    }
}
