using System.Runtime.Serialization;

namespace FeedService.Contract
{
    [DataContract]
    public class Result
    {
        [DataMember]
        public ResultCode Code { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string DisplayMessage { get; set; }
    }
}