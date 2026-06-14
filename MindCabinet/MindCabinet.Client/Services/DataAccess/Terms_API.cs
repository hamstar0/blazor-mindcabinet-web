using System.Net.Http.Json;
using System.Text.Json;
using MindCabinet.Client.Services.DataAccess;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;
using MindCabinet.Shared.Utility;


namespace MindCabinet.Client.Services.DbAccess;



public partial class ClientDataAccess_Terms : IClientDataAccess {
    public interface IAPI : IServerDataAccessAPI {
        public const string BaseRoute = "Terms";



        public string GetByX_Route => "GetByX";
        
        public class GetByX_Return( IEnumerable<TermObject.Raw> terms ) {
            public IEnumerable<TermObject.Raw> Terms { get; set; } = terms;
        }

        public Task<GetByX_Return> GetByIds_Async( IEnumerable<TermId> termIds );



        public class GetByCriteria_Params {
            public string TermPattern { get; set; } = "";

            public TermId? ContextTermId { get; set; }

            public string? ContextTermPattern { get; set; }

            // public PrimitiveOptional<long>? ContextContextTermId { get; } = contextContextTermId;
        }
        
        public Task<GetByX_Return> GetByCriteria_Async( GetByCriteria_Params parameters );



        public class Create_Params {
            public string TermPattern { get; set; } = "";
            public TermId? ContextId { get; set; }
            public TermId? AliasId { get; set; }
        }

        public class Create_Return {
            public bool IsAdded { get; set; }
            public TermObject.Raw TermRaw { get; set; } = null!;
        }

        public Task<Create_Return> Create_Async( Create_Params parameters );
    }
}
