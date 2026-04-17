using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;
using MindCabinet.Shared.DataObjects.Term;


namespace MindCabinet.DataObjects;


public partial class ServerDataObject : IDataObject {
    public class Raw : IRawDataObject {
        public TermId UserConceptTermId { get; set; }
    }


    public ServerDataObject() {
        throw new NotImplementedException( "This class is not meant to be instantiated" );
    }
}
