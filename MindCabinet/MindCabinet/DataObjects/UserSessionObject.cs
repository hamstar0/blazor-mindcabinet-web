using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MindCabinet.Shared.DataObjects;


namespace MindCabinet.DataObjects;


public partial class UserSessionObject : IDataObject {
    public class Raw : IRawDataObject {
        public string Id { get; set; } = "";
        public string LatestIpAddress { get; set; } = "";
        public long SimpleUserId { get; set; }
        public DateTime FirstVisit { get; set; }
        public DateTime LatestVisit { get; set; }
        public int Visits { get; set; }
    }
}
