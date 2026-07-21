using System.Net.Http.Json;
using System.Text.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_Terms : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "Term";



        public string GetByX_Route => "GetByX";
        
        public class GetByX_Return( IEnumerable<TermObject.Raw> terms ) {
            public IEnumerable<TermObject.Raw> Terms { get; set; } = terms;
        }

        public Task<GetByX_Return> GetByIds_Async( IEnumerable<TermId> termIds );



        public class GetByCriteria_Params {
            public string? TermPattern { get; set; } = "";

            public string? AbbrevPattern { get; set; } = "";

            public TermId? ContextTermId { get; set; }

            public string? ContextTermPattern { get; set; }
        }
        
        public Task<GetByX_Return> GetByCriteria_Async( GetByCriteria_Params parameters );



        public class CreateForCurrentUser_Params {
            public string TermBody { get; set; } = "";
            public string? Abbreviation { get; set; } = "";
            public string? Description { get; set; } = "";
            public TermId? ContextId { get; set; }
            public TermId? AliasId { get; set; }
        }

        public class CreateForCurrentUser_Return {
            public bool IsAdded { get; set; }
            public TermObject.Raw TermRaw { get; set; } = null!;
        }

        public Task<CreateForCurrentUser_Return> CreateForCurrentUser_Async( CreateForCurrentUser_Params parameters );


        public class UpdateForCurrentUser_Params {
            public TermId Id { get; set; }
            public string? TermBody { get; set; } = "";
            public string? Abbreviation { get; set; } = "";
            public string? Description { get; set; } = "";
            public TermId? ContextId { get; set; }
            public TermId? AliasId { get; set; }
        }

        public Task<bool> UpdateForCurrentUser_Async( UpdateForCurrentUser_Params parameters );
    }
}
