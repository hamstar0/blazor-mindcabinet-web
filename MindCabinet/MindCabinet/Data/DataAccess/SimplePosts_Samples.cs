using Dapper;
using MindCabinet.Client.Services;
using MindCabinet.Client.Services.DbAccess;
using MindCabinet.Shared.DataObjects;
using System.Data;


namespace MindCabinet.Data.DataAccess;


public partial class ServerDataAccess_SimplePosts : IServerDataAccess {
    private async Task InstallSamples_Async(
                IDbConnection dbConnection,
                ServerDataAccess_Terms termsData,
                ServerDataAccess_Terms_Sets termSetsData,
                long defaultUserId ) {
        ClientDataAccess_Terms.Create_Return term1 = await termsData.Create_Async(
            dbConnection,
            termsData,
            new ClientDataAccess_Terms.Create_Params("Term1", null, null)
        );
        ClientDataAccess_Terms.Create_Return term2 = await termsData.Create_Async(
            dbConnection,
            termsData,
            new ClientDataAccess_Terms.Create_Params("Term2", null, null)
        );
        ClientDataAccess_Terms.Create_Return term3 = await termsData.Create_Async(
            dbConnection,
            termsData,
            new ClientDataAccess_Terms.Create_Params("Term3", null, null)
        );

        var fillerPosts = new List<object>() {
            new {
                Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(25),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(25),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term1.Term, term3.Term)
            },
            new {
                Body = "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(24),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(24),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(23),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(23),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(21),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(21),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(19),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(19),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(18),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(18),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(15),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(15),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term1.Term)
            },
            new {
                Body = "Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?",
                Created = DateTime.UtcNow - TimeSpan.FromHours(11),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(11),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term2.Term)
            },
            new {
                Body = "Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
                Created = DateTime.UtcNow - TimeSpan.FromHours(10),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(10),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term2.Term)
            },
            new {
                Body = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(9),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(9),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                Body = "Et harum quidem rerum facilis est et expedita distinctio.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(8),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(8),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                Body = "Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(7),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(7),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                Body = "Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(5),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(5),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term2.Term, term3.Term)
            },
            new {
                Body = "Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.",
                Created = DateTime.UtcNow - TimeSpan.FromHours(3),
                Modified = DateTime.UtcNow - TimeSpan.FromHours(3),
                SimpleUserId = defaultUserId,
                TermSetId = await termSetsData.Create_Async(dbConnection, term2.Term, term3.Term)
            },
        };

        string sql = $@"INSERT INTO {ServerDataAccess_SimplePosts.TableName}
                    (Body, Created, Modified, SimpleUserId, TermSetId)
                    VALUES (@Body, @Created, @Modified, @SimpleUserId, @TermSetId)";
        int rowsAffected = await dbConnection.ExecuteAsync( sql, fillerPosts );
    }
}
