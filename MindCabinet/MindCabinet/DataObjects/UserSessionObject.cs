using MindCabinet.Shared.Utility;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace MindCabinet.DataObjects;


public partial class UserSessionObject {
    public class Raw {
        public string Id = "";
        public string LatestIpAddress = "";
        public long SimpleUserId;
        public DateTime FirstVisit;
        public DateTime LatestVisit;
        public int Visits;
    }
}
